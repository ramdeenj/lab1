.section .text
    .globl main
    /* ********** main ********** */
main:
    pushq %rbp
    movq %rsp, %rbp
    /* Allocate space for 3 temporaries */
    subq $48, %rsp
    /* return <expr> */
    /* Constant [NUM 3 3] */
    movq $3, %rax
    /* copy register to temporary 0 */
    movq %rax, -8(%rbp)
    /* set storage class of temporary 0 */
    movq $12345, -16(%rbp)
    /* Constant [NUM 3 3] */
    movq $3, %rax
    /* copy register to temporary 1 */
    movq %rax, -24(%rbp)
    /* set storage class of temporary 1 */
    movq $12345, -32(%rbp)
    /* copy temporary 0 value to register */
    movq -8(%rbp), %rax
    /* copy temporary 1 value to register */
    movq -24(%rbp), %rbx
    cmpq %rbx, %rax
    setne %al
    movzbl %al, %eax
    /* copy register to temporary 2 */
    movq %rax, -40(%rbp)
    /* set storage class of temporary 2 */
    movq $12345, -48(%rbp)
    /* copy temporary 2 value to register */
    movq -40(%rbp), %rax
    /* Epilogue */
    movq %rbp, %rsp
    popq %rbp
    ret
    /* ********** End of main ********** */
.section .data
.section .bss
