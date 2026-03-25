// Each temporary uses 16 bytes on the stack:
//   rbp + valueOffset  = 8-byte value
//   rbp + classOffset  = 8-byte storage class
//
// For temp number i:
//   valueOffset = -(i*16 + 8)
//   classOffset = -(i*16 + 16)

public class Temporary
{
    public readonly int number;

    public Temporary(int number)
    {
        this.number = number;
    }

    public int valueOffset => -(number * 16 + 8);
    public int classOffset => -(number * 16 + 16);

    public void moveToRegister(ASM.Register reg)
    {
        ASM.Asm.emit(new ASM.Comment($"copy temporary {number} value to register"));
        ASM.Asm.emit(new ASM.OpMovRegIndReg(valueOffset, ASM.Register.rbp, reg));
    }

    public void moveToXmmRegister(ASM.Register xmm)
    {
        ASM.Asm.emit(new ASM.Comment($"copy float temporary {number} to xmm"));
        ASM.Asm.emit(new ASM.OpMovsdMemXmm(valueOffset, ASM.Register.rbp, xmm));
    }

    public void moveFromRegister(ASM.Register reg, ASM.StorageClass sc)
    {
        ASM.Asm.emit(new ASM.Comment($"copy register to temporary {number}"));
        ASM.Asm.emit(new ASM.OpMovRegRegInd(reg, valueOffset, ASM.Register.rbp));
        ASM.Asm.emit(new ASM.Comment($"set storage class of temporary {number}"));
        ASM.Asm.emit(new ASM.OpMovConstRegInd((long)sc, classOffset, ASM.Register.rbp));
    }

    public void moveFromXmmRegister(ASM.Register xmm, ASM.StorageClass sc)
    {
        ASM.Asm.emit(new ASM.Comment($"copy xmm to float temporary {number}"));
        ASM.Asm.emit(new ASM.OpMovsdXmmMem(xmm, valueOffset, ASM.Register.rbp));
        ASM.Asm.emit(new ASM.Comment($"set storage class of temporary {number}"));
        ASM.Asm.emit(new ASM.OpMovConstRegInd((long)sc, classOffset, ASM.Register.rbp));
    }
}