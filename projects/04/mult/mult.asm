// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/04/Mult.asm

// Multiplies R0 and R1 and stores the result in R2.
// (R0, R1, R2 refer to RAM[0], RAM[1], and RAM[2], respectively.)

// Put your code here.

//A = Addressregister
//D = Dataregister
//M = Currently Loaded Memoryregister
//MD = Currently loaded memoryregister + Dataregister
//0;JMP = Jump to address
//0;JGT = Jump if greater than

    @R2 
    M=0

    @R0 
    D=M

    @End 
    D;JLE 

(LOOP)
    @R1 
    D=M

    @R2
    M=D+M

    @R0
    MD=M-1

    @LOOP
    D;JGT

(End)
    @End
    0;JMP




