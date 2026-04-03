.section .text
    .globl main
    .extern SetErrorMode
    // ********** main **********
main:
    pushq %rbp
    movq %rsp, %rbp
    // Allocate space for 20 temporaries
    subq $320, %rsp
    // Suppress Windows crash dialogs
    movq $0x8007, %rcx
    callq SetErrorMode
    // Constant [NUM 3 1]
    movq $1, %rax
    // copy register to temporary 0
    movq %rax, -8(%rbp)
    // set storage class of temporary 0
    movq $12345, -16(%rbp)
    // Constant [NUM 3 2]
    movq $2, %rax
    // copy register to temporary 1
    movq %rax, -24(%rbp)
    // set storage class of temporary 1
    movq $12345, -32(%rbp)
    // copy temporary 0 value to register
    movq -8(%rbp), %rax
    // copy temporary 1 value to register
    movq -24(%rbp), %rbx
    addq %rbx, %rax
    // copy register to temporary 2
    movq %rax, -40(%rbp)
    // set storage class of temporary 2
    movq $12345, -48(%rbp)
    // Constant [NUM 3 1]
    movq $1, %rax
    // copy register to temporary 3
    movq %rax, -56(%rbp)
    // set storage class of temporary 3
    movq $12345, -64(%rbp)
    // copy temporary 2 value to register
    movq -40(%rbp), %rax
    // copy temporary 3 value to register
    movq -56(%rbp), %rbx
    cmpq %rbx, %rax
    setg %al
    movzbl %al, %eax
    // copy register to temporary 4
    movq %rax, -72(%rbp)
    // set storage class of temporary 4
    movq $12345, -80(%rbp)
    // copy temporary 4 value to register
    movq -72(%rbp), %rax
    testq %rax, %rax
    je lbl0
    // return <expr>
    // Constant [NUM 4 100]
    movq $100, %rax
    // copy register to temporary 5
    movq %rax, -88(%rbp)
    // set storage class of temporary 5
    movq $12345, -96(%rbp)
    // copy temporary 5 value to register
    movq -88(%rbp), %rax
    // Epilogue
    movq %rbp, %rsp
    popq %rbp
    ret
    jmp lbl1
lbl0:
    // Constant [NUM 5 3]
    movq $3, %rax
    // copy register to temporary 6
    movq %rax, -104(%rbp)
    // set storage class of temporary 6
    movq $12345, -112(%rbp)
    // Constant [NUM 5 4]
    movq $4, %rax
    // copy register to temporary 7
    movq %rax, -120(%rbp)
    // set storage class of temporary 7
    movq $12345, -128(%rbp)
    // copy temporary 6 value to register
    movq -104(%rbp), %rax
    // copy temporary 7 value to register
    movq -120(%rbp), %rbx
    addq %rbx, %rax
    // copy register to temporary 8
    movq %rax, -136(%rbp)
    // set storage class of temporary 8
    movq $12345, -144(%rbp)
    // Constant [NUM 5 2]
    movq $2, %rax
    // copy register to temporary 9
    movq %rax, -152(%rbp)
    // set storage class of temporary 9
    movq $12345, -160(%rbp)
    // copy temporary 8 value to register
    movq -136(%rbp), %rax
    // copy temporary 9 value to register
    movq -152(%rbp), %rbx
    cmpq %rbx, %rax
    setg %al
    movzbl %al, %eax
    // copy register to temporary 10
    movq %rax, -168(%rbp)
    // set storage class of temporary 10
    movq $12345, -176(%rbp)
    // copy temporary 10 value to register
    movq -168(%rbp), %rax
    testq %rax, %rax
    je lbl2
    // return <expr>
    // Constant [NUM 6 150]
    movq $150, %rax
    // copy register to temporary 11
    movq %rax, -184(%rbp)
    // set storage class of temporary 11
    movq $12345, -192(%rbp)
    // copy temporary 11 value to register
    movq -184(%rbp), %rax
    // Epilogue
    movq %rbp, %rsp
    popq %rbp
    ret
    jmp lbl3
lbl2:
    // Constant [NUM 7 5]
    movq $5, %rax
    // copy register to temporary 12
    movq %rax, -200(%rbp)
    // set storage class of temporary 12
    movq $12345, -208(%rbp)
    // Constant [NUM 7 6]
    movq $6, %rax
    // copy register to temporary 13
    movq %rax, -216(%rbp)
    // set storage class of temporary 13
    movq $12345, -224(%rbp)
    // copy temporary 12 value to register
    movq -200(%rbp), %rax
    // copy temporary 13 value to register
    movq -216(%rbp), %rbx
    addq %rbx, %rax
    // copy register to temporary 14
    movq %rax, -232(%rbp)
    // set storage class of temporary 14
    movq $12345, -240(%rbp)
    // Constant [NUM 7 3]
    movq $3, %rax
    // copy register to temporary 15
    movq %rax, -248(%rbp)
    // set storage class of temporary 15
    movq $12345, -256(%rbp)
    // copy temporary 14 value to register
    movq -232(%rbp), %rax
    // copy temporary 15 value to register
    movq -248(%rbp), %rbx
    cmpq %rbx, %rax
    setg %al
    movzbl %al, %eax
    // copy register to temporary 16
    movq %rax, -264(%rbp)
    // set storage class of temporary 16
    movq $12345, -272(%rbp)
    // copy temporary 16 value to register
    movq -264(%rbp), %rax
    testq %rax, %rax
    je lbl4
    // return <expr>
    // Constant [NUM 8 200]
    movq $200, %rax
    // copy register to temporary 17
    movq %rax, -280(%rbp)
    // set storage class of temporary 17
    movq $12345, -288(%rbp)
    // copy temporary 17 value to register
    movq -280(%rbp), %rax
    // Epilogue
    movq %rbp, %rsp
    popq %rbp
    ret
    jmp lbl5
lbl4:
    // return <expr>
    // Constant [NUM 10 250]
    movq $250, %rax
    // copy register to temporary 18
    movq %rax, -296(%rbp)
    // set storage class of temporary 18
    movq $12345, -304(%rbp)
    // copy temporary 18 value to register
    movq -296(%rbp), %rax
    // Epilogue
    movq %rbp, %rsp
    popq %rbp
    ret
lbl5:
lbl3:
lbl1:
    // return <expr>
    // Constant [NUM 12 99]
    movq $99, %rax
    // copy register to temporary 19
    movq %rax, -312(%rbp)
    // set storage class of temporary 19
    movq $12345, -320(%rbp)
    // copy temporary 19 value to register
    movq -312(%rbp), %rax
    // Epilogue
    movq %rbp, %rsp
    popq %rbp
    ret
    // ********** End of main **********
.section .data
.section .bss
