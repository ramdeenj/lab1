.section .text
    .globl main
    .extern SetErrorMode
    .extern _rtinit
    .extern _putc
    .extern _newline
    .extern _putv
    .extern _getc
    .extern _print
    .extern _length
main:
    pushq %rbp
    movq %rsp, %rbp
    subq $128, %rsp
    movq $0x8007, %rcx
    callq SetErrorMode
    subq $32, %rsp
    callq _rtinit
    addq $32, %rsp
    leaq emptyString(%rip), %rax
    movq %rax, -120(%rbp)
    leaq strconst0(%rip), %rax
    // copy register to temporary 1
    movq %rax, -24(%rbp)
    // set storage class of temporary 1
    movq $12345, -32(%rbp)
    // copy temporary 1 value to register
    movq -24(%rbp), %rax
    movq %rax, -120(%rbp)
    // copy register to temporary 2
    movq %rax, -40(%rbp)
    // set storage class of temporary 2
    movq $12345, -48(%rbp)
    movq -120(%rbp), %rax
    // copy register to temporary 4
    movq %rax, -72(%rbp)
    // set storage class of temporary 4
    movq $12345, -80(%rbp)
    movq $12345, %rax
    pushq %rax
    // copy temporary 4 value to register
    movq -72(%rbp), %rax
    pushq %rax
    movq %rsp, %rcx
    subq $32, %rsp
    callq _print
    addq $32, %rsp
    addq $16, %rsp
    // copy register to temporary 5
    movq %rax, -88(%rbp)
    // set storage class of temporary 5
    movq $12345, -96(%rbp)
    // Constant [NUM 6 1]
    movq $1, %rax
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
    movq %rbp, %rsp
    popq %rbp
    ret
emptyString:
    .quad 0
strconst0:
    .quad 22
    .byte 0x74, 0x68, 0x65, 0x20, 0x2f, 0x2a, 0x20, 0x74, 0x65, 0x78, 0x74, 0x20, 0x2a, 0x2f, 0x0a, 0x69, 0x6e, 0x74, 0x33, 0x20, 0x2f, 0x2a
    .byte 0x00, 0x00
.section .data
.section .bss
