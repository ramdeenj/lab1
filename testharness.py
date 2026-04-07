#!/usr/bin/python

VERBOSE=False

TIMEOUT = 10

#number of tests to skip so you can go right to the failing one
SKIP=0

COMPILER="bin/Debug/net8.0/lab1.exe"

import shlex, subprocess, sys, time, json, getopt, tempfile, os, os.path, platform

class BooBoo(Exception):
    pass

RUN_TIMEOUT=3

def run(args,quiet,timeout=None):

    if timeout == None:
        timeout = TIMEOUT

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
        o,e = p.communicate(timeout=timeout)
        if not o:
            o=b""
        if not e:
            e=b""

        o=o.decode(errors="ignore")
        e=e.decode(errors="ignore")
        completed=True
        rcode = p.returncode
        if rcode < 0:
            #posix signal; simulate windows error
            rcode = 0x40000000
        rcode &= 0xffffffff
        if rcode >= 0x40000000 and rcode <= 0xfffffc00:
            crashed = True
        else:
            crashed = False

    except subprocess.TimeoutExpired:
        p.kill()
        completed=False
        crashed=False
        rcode=-1
        o=""
        e=""

    return completed,crashed,rcode, o, e


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



def fatal(*msg):
    tmp = " ".join([str(q) for q in msg])
    print("ERROR!")
    print(tmp)
    sys.exit(1)

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

    def badNonbonus(*msg):
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

    def bad(*msg):
        nonlocal isBonus
        if isBonus:
            badBonus(*msg)
        else:
            badNonbonus(*msg)

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
            stopOnFirstFail=False
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
        fatal("Could not find tests folder")

    numtests = 0
    alltests=[]
    for dirpath,dirs,files in os.walk(f"tests/inputs"):
        for f in files:
            if f.endswith(".txt"):
                alltests.append( (dirpath,f) )

    alltests.sort()

    if len(alltests) == 0:
        fatal("No tests found")

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

        shouldcompile = J.pop("compiles",True)
        isBonus = J.pop("bonus",False)
        shouldComplete = not J.pop("hangs",False)
        shouldCrash = J.pop("crash",False)
        shouldReturn=J.pop("returns",None)
        output=J.pop("output",None)
        input=J.pop("input",None)
        reason=J.pop("reason","")
        if reason:
            reason = " "+reason

        if J:
            print(J)
            assert 0

        if isBonus:
            testText = "Bonus test"
        else:
            testText = "Test"

        flags=[]
        if not shouldcompile:
            flags.append("invalid")
        if isBonus:
            flags.append("bonus")
        if not shouldComplete:
            flags.append("hangs")
        if shouldCrash:
            flags.append("crashes")

        if len(flags):
            flags="[" + ",".join(flags) + f"{reason}]"
        else:
            flags=""

        print(f"{testText} {counter+1} of {len(alltests)} ({f}) {flags}...",end="")
        if counter < SKIP:
            print("SKIPPED")
            continue
        else:
            print()


        completed,crashed,r,o,e = run([COMPILER,dirpath+"/"+f,"out.asm"],quiet=False)
        if not completed:
            bad("Compiler froze")
        if crashed:
            didcompile = False
        else:
            didcompile = (r==0)

        if shouldcompile != didcompile:
            msg="Program "
            if shouldcompile:
                msg += "should have compiled "
            else:
                msg += "should not have compiled "
            if didcompile:
                msg += "but it did compile"
            else:
                msg += "but it did not compile"
            bad(msg)
        else:
            if didcompile:

                didComplete,didCrash,didReturn,o,e = run([os.path.join(".","out.exe")],quiet=True,timeout=RUN_TIMEOUT)

                if shouldComplete != didComplete:
                    msg="Executable should have "
                    if shouldComplete:
                        msg += "completed "
                    else:
                        msg += "hung "
                    msg += "but it "
                    if didComplete:
                        msg += "completed"
                    else:
                        msg += "hung"
                    bad(msg)
                else:
                    if shouldCrash != didCrash:
                        msg="Executable should "
                        if shouldCrash:
                            pass
                        else:
                            msg += "not "
                        msg += "have crashed but it "
                        if didCrash:
                            msg += "did "
                        else:
                            msg += "not "
                        msg += "crash"
                        bad(msg)
                    else:
                        if shouldReturn == None:
                            shouldReturn=[-1]
                            didReturn=-1

                        if type(shouldReturn) == int:
                            shouldReturn = [shouldReturn]
                        elif type(shouldReturn) == list:
                            pass
                        elif shouldReturn == None:
                            pass
                        else:
                            assert 0,f"shouldReturn is {shouldReturn}"

                        if didReturn not in shouldReturn:
                            if len(shouldReturn) > 1:
                                X = ",".join([str(q) for q in shouldReturn])
                                shouldReturn = "one of {" + str(X) + "}"
                            else:
                                shouldReturn = shouldReturn[0]

                            msg = f"Executable should have returned {shouldReturn} but it returned {didReturn}"
                            bad(msg)
                        else:
                            good()
                        #end if return matches
                    #end if should crash
                #end if should complete
            else:
                #should not compile and did not compile
                good()
            #end if did compile
        #end if should compile != did compile
    #end for each test

    print("Done. Out of",len(alltests),"tests:")
    printStats()



try:
    main()
except BooBoo:
    pass
