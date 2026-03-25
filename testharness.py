#!/usr/bin/python

VERBOSE=False

TIMEOUT = 5

#number of tests to skip so you can go right to the failing one
SKIP=0

COMPILER="bin/Debug/net8.0/lab1.exe"


import shlex, subprocess, sys, time, json, getopt, tempfile, os, os.path, platform

class BooBoo(Exception):
    pass

def run(args,quiet):

    if VERBOSE:
        print(" ".join( [shlex.quote(q) for q in args] ) )

    kw={}

    if quiet:
        kw["stdout"] = subprocess.PIPE
        kw["stderr"] = subprocess.PIPE

    if "win" in platform.system().lower():
        kw["creationflags"] = subprocess.CREATE_NO_WINDOW

    p = subprocess.Popen(args,**kw)

    try:
        o,e = p.communicate(timeout=TIMEOUT)
        if not o:
            o=b""
        if not e:
            e=b""

        o=o.decode(errors="ignore")
        e=e.decode(errors="ignore")

    except subprocess.TimeoutExpired:
        p.kill()
        raise

    return p.returncode, o, e


def replace(lst, values):
    if type(lst) == str:
        lst=[lst]
    if type(values) == str:
        values=[values]

    repl=[]
    for item in lst:
        if item == "{}":
            repl += values
        else:
            repl.append(item)
    return repl

    if p.returncode != 0:
        print(f"Process {args[0]} exited with code {p.returncode}")
        sys.exit(1)



def error(*msg):
    tmp = " ".join([str(q) for q in msg])
    print("ERROR!")
    print(tmp)
    raise BooBoo()

def main():
    stopOnFirstFail=True
    stopOnFirstFailBonus=False

    global VERBOSE
    global SKIP
    global COMPILER

    numGood=0
    numBad=0
    numGoodBonus=0
    numBadBonus=0

    def good():
        nonlocal numGood
        numGood+=1
        print("OK")
    def goodBonus():
        nonlocal numGoodBonus
        numGoodBonus+=1
        print("OK")

    def bad(*msg):
        nonlocal numBad, stopOnFirstFail
        numBad+=1
        tmp = " ".join([str(q) for q in msg])
        print("ERROR")
        print(tmp)
        if stopOnFirstFail:
            printStats()
            sys.exit(1)

    def badBonus(*msg):
        nonlocal numBadBonus
        numBadBonus+=1
        tmp = " ".join([str(q) for q in msg])
        print("ERROR")
        print(tmp)
        if stopOnFirstFailBonus:
            printStats()
            sys.exit(1)

    def printStats():
        print("Num Passing:      ",numGood)
        if numGoodBonus + numBadBonus > 0:
            print("Num Bonus Passing:",numGoodBonus)
        print("Num Failing:      ",numBad)
        if numGoodBonus + numBadBonus > 0:
            print("Num Bonus Failing:",numBadBonus)




    opts,args = getopt.gnu_getopt(sys.argv[1:],"kvs:b",
        [ "--stop", "--verbose", "--skip","--stopbonus" ]
    )

    for o,a in opts:
        if o in ["-k","--stop"]:
            stopOnFirstFail=not stopOnFirstFail
        elif o in ["-v","--verbose"]:
            VERBOSE=True
        elif o in ["-s","--skip"]:
            SKIP=int(a)
        elif o in ["-b","--stopbonus"]:
            stopOnFirstFailBonus=True
        else:
            assert 0,f"{o} is not a valid argument"

    if args:
        COMPILER=args[0]

    if not os.path.exists("tests") or not os.path.exists("tests/inputs"):
        error("Could not find tests folder")

    numtests = 0
    alltests=[]
    for dirpath,dirs,files in os.walk(f"tests/inputs"):
        for f in files:
            if f.endswith(".txt"):
                alltests.append( (dirpath,f) )

    alltests.sort()

    if len(alltests) == 0:
        error("No tests found")

    for counter,tmp in enumerate(alltests):
        dirpath,f = tmp


        with open(f"{dirpath}/{f}") as fp:
            jdata=[]
            while True:
                line = fp.readline().strip()
                if not line.startswith("//"):
                    break
                jdata.append(line[2:].strip())

        try:
            J = json.loads( " ".join(jdata), parse_int=lambda x: int(x,0) )
        except json.decoder.JSONDecodeError as e:
            print("Invalid JSON in",f,":")
            print("".join(jdata))
            print(e)
            sys.exit(1)

        shouldcompile = J.get("compiles",True)
        isBonus = J.get("bonus",False)

        if isBonus:
            testText = "Bonus test"
        else:
            testText = "Test"

        print(f"{testText} {counter+1} of {len(alltests)} ({f})...",end="")
        if counter < SKIP:
            print("SKIPPED")
            continue
        else:
            print()


        r,o,e = run([COMPILER,dirpath+"/"+f,"out.asm"],quiet=False)
        didcompile = (r==0)

        if shouldcompile:
            if didcompile:
                try:
                    r,o,e = run([os.path.join(".","out.exe")],quiet=True)
                    if "returns" in J:
                        if type(J["returns"]) != list:
                            rv = [ J["returns"] ]
                        else:
                            rv = J["returns"]
                        for candidate in rv:
                            if r == candidate:
                                if isBonus:
                                    goodBonus()
                                else:
                                    good()
                                break
                        else:
                            if type(J["returns"]) == list:
                                msg = f"one of: {J['returns']}"
                            else:
                                msg=f"{J['returns']}"
                            if isBonus:
                                badBonus(f"Executable returned {r} but should have returned {msg}")
                            else:
                                bad(f"Executable returned {r} but should have returned {msg}")

                except BooBoo:
                    if isBonus:
                        badBonus("Compile failed")
                    else:
                        bad("Compile failed")
            else:
                if isBonus:
                    badBonus("File should compile without error but it did not")
                else:
                    bad("File should compile without error but it did not")
        else:
            if didcompile:
                if isBonus:
                    badBonus("File should not compile but it did")
                else:
                    bad("File should not compile but it did")
            else:
                if isBonus:
                    goodBonus()
                else:
                    good()



    print("Done. Out of",len(alltests),"tests:")
    printStats()



try:
    main()
except BooBoo:
    pass
