import subprocess
import os

TEST_DIR = "tests/inputs"

tests = [
    "t1.txt",
    "t2.txt",
    "t3.txt",
    "t4.txt",
    "t5.txt",
    "t6.txt",
    "t7.txt",
    "t8.txt",
    "t9.txt",
    "t10.txt",
    "t11.txt",
]

for test in tests:
    path = os.path.join(TEST_DIR, test)

    print("=" * 50)
    print(f"Running {test}")
    print("=" * 50)

    result = subprocess.run(
        ["dotnet", "run", "--", path],
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        encoding="utf-8",
        errors="replace"
    )

    if result.stdout:
        print("STDOUT:")
        print(result.stdout)

    if result.stderr:
        print("STDERR:")
        print(result.stderr)
