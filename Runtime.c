typedef void* HANDLE;
typedef unsigned DWORD;
typedef signed long long int64_t;

_Static_assert(sizeof(DWORD) == 4, "DWORD bad");
_Static_assert(sizeof(int64_t) == 8, "int64_t bad");

typedef struct StackVar_ {
    int64_t value;
    int64_t storageClass;
} StackVar;

static HANDLE stdin_h;
static HANDLE stdout_h;

extern __attribute__((ms_abi)) HANDLE GetStdHandle(DWORD n);
extern __attribute__((ms_abi)) int WriteFile(HANDLE h, void* buf, DWORD count, DWORD* numWritten, void* overlapped);
extern __attribute__((ms_abi)) int ReadFile(HANDLE h, void* buf, DWORD count, DWORD* numRead, void* overlapped);

__attribute__((ms_abi))
void _rtinit() {
    stdin_h  = GetStdHandle(0xfffffff6);
    stdout_h = GetStdHandle(0xfffffff5);
}

__attribute__((ms_abi))
int64_t _putc(StackVar* stk) {
    DWORD count;
    char v = (char)stk[0].value;
    return WriteFile(stdout_h, &v, 1, &count, (void*)0);
}

__attribute__((ms_abi))
void _newline(StackVar* stk) {
    DWORD count;
    char v = '\n';
    WriteFile(stdout_h, &v, 1, &count, (void*)0);
}

static unsigned toHex(int64_t x, char output[16]) {
    unsigned shiftcount = 60, oo = 0;
    const char* digits = "0123456789abcdef";
    if (x == 0) { output[0] = '0'; return 1; }
    for (unsigned i = 0; i < 16; ++i) {
        unsigned j = (unsigned)((x >> shiftcount) & 0xf);
        if (oo > 0 || j) output[oo++] = digits[j];
        shiftcount -= 4;
    }
    return oo;
}

static unsigned toDecimal(int64_t x, char output[20]) {
    if (x == 0) { *output = '0'; return 1; }
    int64_t place = (int64_t)10000000000000000000ULL;
    int oo = 0;
    while (place > 0) {
        int64_t quotient = x / place;
        if (quotient || oo > 0) output[oo++] = '0' + (int)quotient;
        x = x - quotient * place;
        place = place / 10;
    }
    return oo;
}

static unsigned toBin(int64_t number, char output[64]) {
    int64_t mask = (int64_t)0x8000000000000000ULL;
    if (number == 0) { output[0] = '0'; return 1; }
    int oo = 0;
    for (int i = 0; i < 64; ++i, mask >>= 1) {
        if (mask & number) output[oo++] = '1';
        else if (oo > 0)   output[oo++] = '0';
    }
    return oo;
}

__attribute__((ms_abi))
int64_t _putv(StackVar* stk) {
    int64_t x    = stk[0].value;
    int64_t base = stk[1].value;
    char buf[64];
    unsigned len;
    if      (base == 16) len = toHex(x, buf);
    else if (base == 10) len = toDecimal(x, buf);
    else if (base == 2)  len = toBin(x, buf);
    else return 0;
    DWORD count;
    WriteFile(stdout_h, buf, (DWORD)len, &count, (void*)0);
    return 1;
}

__attribute__((ms_abi))
int64_t _getc(StackVar* stk) {
    DWORD count;
    char v;
    ReadFile(stdin_h, &v, 1, &count, (void*)0);
    return (int64_t)v;
}