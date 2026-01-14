#!/usr/bin/env python


#set this equal to the path to your compiler executable
#or else use the -c command line option
COMPILER = "lab1.exe"

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
    P = subprocess.Popen([COMPILER,inp],stdout=subprocess.PIPE)
    o,e = P.communicate()
    o = o.decode(errors="ignore")
    if P.returncode != 0:
        return "null"
    else:
        return o

def compare(actual,expected):
    if expected == None and actual == None:
        return True,True
        
    if expected == None and actual != None:
        return False,False
        
    if expected != None and actual == None:
        return False,False

    if type(actual) != list :
        print("Expected a list; got",type(actual))
        return False,False
        
    if len(actual) != len(expected):
        print("Expected list to have",len(expected),"tokens, but got",len(actual),"tokens")
        return False,False
    bonusOK=True
    for i in range(len(expected)):
        atoken = actual[i]
        etoken = expected[i]
        if atoken["sym"] != etoken["sym"]:
            print("Token",i,": Expected sym to be",etoken["sym"],"but got",atoken["sym"])
            return False,False
        if atoken["line"] != etoken["line"]:
            print("Token",i,": Expected line to be",etoken["line"],"but got",atoken["line"])
            return False,False
        if atoken["lexeme"] != etoken["lexeme"]:
            print("Token",i,": Expected lexeme to be",etoken["lexeme"],"but got",atoken["lexeme"])
            return False,False
        if atoken.get("column") != etoken["column"]:
            bonusOK=False
    return True,bonusOK



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
    allBonusOK=True
    if i >= numToSkip:
        inputfile = os.path.join(dirname,fname)
        output = run(inputfile)
        outputJ = json.loads(output)
        with open(os.path.join(outputfolder,fname)) as fp:
            data = fp.read()
        expected = json.loads(data)
        requiredOK, bonusOK = compare(outputJ,expected)
        if allBonusOK and not bonusOK:
            allBonusOK=False
            print("NOTE: Bonus not matched")
        if not requiredOK:
            error("Mismatch")
    else:
        pass

print("All required functionality OK")
if allBonusOK:
    print("Bonus functionality OK")
done()
