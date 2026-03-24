#!/usr/bin/env python


#set this equal to the path to your compiler executable
#or else use the -c command line option
COMPILER="bin/Debug/net8.0/lab1.exe"

#can skip over some tests to get right to a specific one
#Edit this or use the -s command line option
numToSkip=0

#to run noninteractively, set this to False or use the -n command line option
interactive=True


import sys, os.path, getopt, subprocess, json, re


def error(msg):
    print(msg)
    with open(inputfile) as fp:
        data = fp.read()

    print("=============")
    print(data)
    print("=============")
    if interactive:
        input("Press 'enter' to quit. ")
    sys.exit(0)

def done():
    if interactive:
        input("Press 'enter' to quit. ")
    sys.exit(0)

def run(inp):
    P = subprocess.Popen([COMPILER,inp],stdout=subprocess.PIPE)
    o,e = P.communicate()
    return P.returncode,o.decode()

 
def makeSet(txt):
    lines = txt.strip().split("\n")
    s=set()
    for line in lines:
        line=line.strip().lower()
        if "on line " in line:
            s.add(line)
    return s

def areSame(expected,actual):
    eset = makeSet(expected)
    aset = makeSet(actual)
    if eset == aset:
        return True

    print("Mismatch!")
    missing = eset-aset
    if len(missing):
        print()
        print("These lines were expected but were not found:")
        print("=============================================")
        for m in missing:
            print(m)
    extra = aset-eset
    if len(extra):
        print()
        print("These lines were not expected:")
        print("==============================")
        for e in extra:
            print(e)
    return False


opts,args = getopt.getopt(sys.argv[1:], "c:ns:" )
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

if len(inputs) == 0:
    print("Did not find any inputs?")
    sys.exit(1)

numPassed=0
numFailed=0
for i in range(len(inputs)):
    if numFailed > 0:
        print()
        print("At least one test failed. Stopping.")
        sys.exit(1)

    dirname,fname = inputs[i]
    print("Test",i+1,"of",len(inputs),"(",fname,")...")
    if i >= numToSkip:
        inputfile = os.path.join(dirname,fname)
        rv,actual = run(inputfile)
        alegal = (rv==0)
        with open(os.path.join("tests","outputs",fname)) as fp:
            expected = fp.read()

        if expected.strip() == "INVALID":
            elegal=False
        else:
            elegal=True

        if alegal == True and elegal == True:
            if areSame(expected,actual):
                numPassed+=1
            else:
                numFailed+=1
        elif alegal == True and elegal == False:
            error("Expected failure, but compiler succeeded")
            numFailed+=1
        elif alegal == False and elegal == True:
            error("Expected success, but compiler failed")
            numFailed+=1
        elif alegal == False and elegal == False:
            numPassed+=1
    else:
        print("Skipping")

assert numPassed+numFailed == len(inputs)
print(numPassed,"tests passed")
print(numFailed,"tests failed")

done()
