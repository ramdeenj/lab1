.section .text
    .globl main
    .extern SetErrorMode
    .extern _rtinit
    .extern _putc
    .extern _newline
    .extern _putv
    .extern _getc
main:
    pushq %rbp
    movq %rsp, %rbp
    subq $144, %rsp
    movq $0x8007, %rcx
    callq SetErrorMode
    subq $32, %rsp
    callq _rtinit
    addq $32, %rsp
lbl2:
    // Constant [NUM 6 5]
    movq $5, %rax
    // copy register to temporary 1
    movq %rax, -24(%rbp)
    // set storage class of temporary 1
    movq $12345, -32(%rbp)
    // copy temporary 1 value to register
    movq -24(%rbp), %rax
    movq %rax, -136(%rbp)
    // copy register to temporary 2
    movq %rax, -40(%rbp)
    // set storage class of temporary 2
    movq $12345, -48(%rbp)
    // Bool constant false
    movq $0, %rax
    // copy register to temporary 3
    movq %rax, -56(%rbp)
    // set storage class of temporary 3
    movq $12345, -64(%rbp)
    // copy temporary 3 value to register
    movq -56(%rbp), %rax
    testq %rax, %rax
    je lbl3
    // Constant [NUM 8 4]
    movq $4, %rax
    // copy register to temporary 4
    movq %rax, -72(%rbp)
    // set storage class of temporary 4
    movq $12345, -80(%rbp)
    // copy temporary 4 value to register
    movq -72(%rbp), %rax
    movq $12345, %rbx
    movq %rbp, %rsp
    popq %rbp
    ret
    // Bool constant true
    movq $1, %rax
    // copy register to temporary 5
    movq %rax, -88(%rbp)
    // set storage class of temporary 5
    movq $12345, -96(%rbp)
    // copy temporary 5 value to register
    movq -88(%rbp), %rax
    testq %rax, %rax
    je lbl4
    jmp lbl0
lbl4:
lbl3:
    // Constant [NUM 13 2]
    movq $2, %rax
    // copy register to temporary 6
    movq %rax, -104(%rbp)
    // set storage class of temporary 6
    movq $12345, -112(%rbp)
    // copy temporary 6 value to register
    movq -104(%rbp), %rax
    movq $12345, %rbx
    movq %rbp, %rsp
    popq %rbp
    ret
lbl0:
    // Bool constant false
    movq $0, %rax
    // copy register to temporary 7
    movq %rax, -120(%rbp)
    // set storage class of temporary 7
    movq $12345, -128(%rbp)
    // copy temporary 7 value to register
    movq -120(%rbp), %rax
    testq %rax, %rax
    je lbl2
lbl1:
    movq %rbp, %rsp
    popq %rbp
    ret
.section .data
.section .bss
