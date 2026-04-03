.section .text
    .globl main
    .extern SetErrorMode
    // ********** main **********
main:
    pushq %rbp
    movq %rsp, %rbp
    // Allocate 21 temporaries + 32 bytes for locals
    subq $368, %rsp
    // Suppress Windows crash dialogs
    movq $0x8007, %rcx
    callq SetErrorMode
    // Constant [NUM 6 1]
    movq $1, %rax
    // copy register to temporary 1
    movq %rax, -24(%rbp)
    // set storage class of temporary 1
    movq $12345, -32(%rbp)
    // copy temporary 1 value to register
    movq -24(%rbp), %rax
    movq %rax, -344(%rbp)
    // copy register to temporary 2
    movq %rax, -40(%rbp)
    // set storage class of temporary 2
    movq $12345, -48(%rbp)
    // Constant [NUM 7 2]
    movq $2, %rax
    // copy register to temporary 4
    movq %rax, -72(%rbp)
    // set storage class of temporary 4
    movq $12345, -80(%rbp)
    // copy temporary 4 value to register
    movq -72(%rbp), %rax
    movq %rax, -352(%rbp)
    // copy register to temporary 5
    movq %rax, -88(%rbp)
    // set storage class of temporary 5
    movq $12345, -96(%rbp)
    // Bool constant true
    movq $1, %rax
    // copy register to temporary 6
    movq %rax, -104(%rbp)
    // set storage class of temporary 6
    movq $12345, -112(%rbp)
    // copy temporary 6 value to register
    movq -104(%rbp), %rax
    testq %rax, %rax
    je lbl0
    // Constant [NUM 10 4]
    movq $4, %rax
    // copy register to temporary 8
    movq %rax, -136(%rbp)
    // set storage class of temporary 8
    movq $12345, -144(%rbp)
    // copy temporary 8 value to register
    movq -136(%rbp), %rax
    movq %rax, -368(%rbp)
    // copy register to temporary 9
    movq %rax, -152(%rbp)
    // set storage class of temporary 9
    movq $12345, -160(%rbp)
    // Load variable 'x'
    movq -368(%rbp), %rax
    // copy register to temporary 11
    movq %rax, -184(%rbp)
    // set storage class of temporary 11
    movq $12345, -192(%rbp)
    // copy temporary 11 value to register
    movq -184(%rbp), %rax
    movq %rax, -360(%rbp)
    // copy register to temporary 12
    movq %rax, -200(%rbp)
    // set storage class of temporary 12
    movq $12345, -208(%rbp)
lbl0:
    // Load variable 'x'
    movq -344(%rbp), %rax
    // copy register to temporary 14
    movq %rax, -232(%rbp)
    // set storage class of temporary 14
    movq $12345, -240(%rbp)
    // Load variable 'y'
    movq -352(%rbp), %rax
    // copy register to temporary 15
    movq %rax, -248(%rbp)
    // set storage class of temporary 15
    movq $12345, -256(%rbp)
    // copy temporary 14 value to register
    movq -232(%rbp), %rax
    // copy temporary 15 value to register
    movq -248(%rbp), %rbx
    addq %rbx, %rax
    // copy register to temporary 16
    movq %rax, -264(%rbp)
    // set storage class of temporary 16
    movq $12345, -272(%rbp)
    // Load variable 'z'
    movq -360(%rbp), %rax
    // copy register to temporary 17
    movq %rax, -280(%rbp)
    // set storage class of temporary 17
    movq $12345, -288(%rbp)
    // copy temporary 16 value to register
    movq -264(%rbp), %rax
    // copy temporary 17 value to register
    movq -280(%rbp), %rbx
    addq %rbx, %rax
    // copy register to temporary 18
    movq %rax, -296(%rbp)
    // set storage class of temporary 18
    movq $12345, -304(%rbp)
    // copy temporary 18 value to register
    movq -296(%rbp), %rax
    movq %rax, -360(%rbp)
    // copy register to temporary 19
    movq %rax, -312(%rbp)
    // set storage class of temporary 19
    movq $12345, -320(%rbp)
    // return <expr>
    // Load variable 'z'
    movq -360(%rbp), %rax
    // copy register to temporary 20
    movq %rax, -328(%rbp)
    // set storage class of temporary 20
    movq $12345, -336(%rbp)
    // copy temporary 20 value to register
    movq -328(%rbp), %rax
    // Epilogue
    movq %rbp, %rsp
    popq %rbp
    ret
    // ********** End of main **********
.section .data
.section .bss
