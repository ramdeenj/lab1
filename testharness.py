#testharness.py

#!/usr/bin/env python


#set this equal to the path to your compiler executable
#or else use the -c command line option
COMPILER="bin/Debug/net8.0/lab1.exe"

#can skip over some tests to get right to a specific one
#Edit this or use the -s command line option
numToSkip=0

#to run noninteractively, set this to False or use the -n command line option
interactive=True


import sys, os.path, getopt, subprocess, json


def error(msg):
    print(msg)
    if interactive:
        input("Press 'enter' to quit. ")
    sys.exit(0)

def done():
    if interactive:
        input("Press 'enter' to quit. ")
    sys.exit(0)

def run(inp):
    P = subprocess.Popen([COMPILER,inp])
    o,e = P.communicate()
    return P.returncode

def compare(expected,actual):
    if expected["token"] != actual["token"]:
        print("Expected node symbol to be",expected["token"],"but it was",actual[symbolKey])
        return False

    if len(expected["children"]) != len(actual["children"]):
        print("Child length mismatch for",expected["token"],": Expected",len(expected["children"]),"but got",len(actual["children"]))
        return False

    for i in range(len(expected["children"])):
        ok = compare(expected["children"][i], actual["children"][i])
        if not ok:
            return False

    return True


opts,args = getopt.getopt(sys.argv[1:], "c:n:s:" )
for o,a in opts:
    if o == "-c":
        COMPILER=a
    elif o == "-n":
        interactive=False
    elif o == "-s":
        numToSkip = int(a)
    else:
        assert False

inputfolder=os.path.join(os.path.dirname(__file__),"tests","inputs")
outputfolder=os.path.join(os.path.dirname(__file__),"tests","outputs")

if not os.path.exists(inputfolder):
    error("Cannot find tests folder; it should be side-by-side with this harness.")

inputs=[]
for dirname,dirs,files in os.walk(inputfolder):
    for f in files:
        if f.endswith(".txt"):
            inputs.append( ( dirname,f) )

inputs.sort()

numPassed=0
numFailed=0
for i in range(len(inputs)):
    dirname,fname = inputs[i]
    print("Test",i,"(",fname,")...")
    if i >= numToSkip:
        inputfile = os.path.join(dirname,fname)
        rv = run(inputfile)
        expectedfile = os.path.join("tests","outputs",fname.replace(".txt",".json"))
        if not os.path.exists(expectedfile):
            if rv == 0:
                error("Expected failure, but compiler succeeded")
            else:
                numPassed += 1
                continue
        else:
            if rv != 0:
                error("Expected success, but compiler failed")
            else:
                with open(expectedfile) as fp:
                    expectedJ = json.load(fp)
                with open("tree.json") as fp:
                    actualJ = json.load(fp)
                if not compare(expectedJ,actualJ):
                    error("Mismatch")
                else:
                    numPassed+=1
                    pass

print(numPassed,"tests passed")
print(numFailed,"tests failed")

done()
