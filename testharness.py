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
    global VERBOSE
    global SKIP
    global COMPILER

    numGood=0
    numBad=0

    def good():
        nonlocal numGood
        numGood+=1
        print("OK")
    def bad(*msg):
        nonlocal numBad, stopOnFirstFail
        numBad+=1
        tmp = " ".join([str(q) for q in msg])
        print("ERROR")
        print(tmp)
        if stopOnFirstFail:
            print("Num Passing:",numGood)
            print("Num Failing:",numBad)
            raise BooBoo()




    opts,args = getopt.gnu_getopt(sys.argv[1:],"kvs:",
        [ "--stop", "--verbose", "--skip" ]
    )


    for o,a in opts:
        if o in ["-k","--stop"]:
            stopOnFirstFail=not stopOnFirstFail
        elif o in ["-v","--verbose"]:
            VERBOSE=True
        elif o in ["-s","--skip"]:
            SKIP=int(a)
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
        print(f"Test {counter+1} of {len(alltests)} ({f})...",end="")
        if counter < SKIP:
            print("SKIPPED")
            continue
        else:
            print()

        tf = tempfile.NamedTemporaryFile(mode="w",delete=False)
        try:
            with open(f"{dirpath}/{f}") as fp:
                jdata=[]
                while True:
                    line = fp.readline().strip()
                    jdata.append(line)
                    tf.write("\n")
                    if len(line) == 0:
                        break
                tf.write(fp.read())
            tf.close()
            try:
                J = json.loads( "".join(jdata) )
            except json.decoder.JSONDecodeError:
                print("Invalid JSON:")
                print("".join(jdata))
                sys.exit(1)

            shouldcompile = J.get("compiles",True)
            r,o,e = run([COMPILER,tf.name,"out.asm"],quiet=False)
            didcompile = (r==0)

            if shouldcompile:
                if didcompile:
                    try:
                        r,o,e = run([os.path.join(".","out.exe")],quiet=True)
                        if "returns" in J and r != J["returns"]:
                            bad(f"Executable returned {r} but should have returned {J['returns']}")
                        else:
                            good()
                    except BooBoo:
                        bad("Resulting assembly language file was invalid")
                else:
                    bad("File should compile without error but it did not")
            else:
                if didcompile:
                    bad("File should not compile but it did")
                else:
                    good()

        finally:
            os.remove(tf.name)


    print("Done. Out of",len(alltests),"tests:")
    print("Num Passing:",numGood)
    print("Num Failing:",numBad)


try:
    main()
except BooBoo:
    pass
