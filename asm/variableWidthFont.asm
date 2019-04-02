 ; F-Zero Climax Character - Story Screen VWF by Normmatt

.gba				; Set the architecture to GBA
.open "rom/output.gba",0x08000000		; Open input.gba for output.
					; 0x08000000 will be used as the
					; header size
					
.relativeinclude on
					
.macro adr,destReg,Address
here:
	.if (here & 2) != 0
		add destReg, r15, (Address-here)-2
	.else
		add destReg, r15, (Address-here)
	.endif
.endmacro

.thumb

.org 0x080433D4
.area 0x080433E8 - 0x080433D4
	mov r0, #13
	add r0,r15
	mov r14, r0
    ldr r0, =HandleCharacter+1    ; r2 is best variable to use for jump
    bx r0
.pool
.endarea

.org 0x080433F4
	nop

.org 0x08DCA900
.thumb
HandleCharacter:
	LDRB    R0, [R5]		   	;ORIGINAL CODE
	
	cmp r0, #0x1F
	bls HandleCharacter_NoneASCII
	
	cmp r0, #0x7D
	bhi HandleCharacter_NoneASCII
	
	;If it makes it here then its within the ASCII range
	ldr r3, =WidthTable-0x20
	ldrb r3, [r3,r0]
	;mov r3, #0x08 ; Width
	
	LDRH    R1, [R4,#0x10]
	ADD     R1, R1, R3
	mov r2, #1
	and r1, r2
	add r3, r3, r1 ;Force alignment if its out
	strb r3, [r4,#2] ; Store current width
	add r5, #1
	b HandleCharacter_Continue
	
HandleCharacter_NoneASCII:
	LSL     R0, R0, #8		   	;ORIGINAL CODE
	LDRB    R3, [R5,#1]		   	;ORIGINAL CODE
	ORR     R0, R3		   		;ORIGINAL CODE
	LDR     R3, =0xFFFF7FC0		;ORIGINAL CODE
	ADD     R0, R0, R3		   	;ORIGINAL CODE
	
	;If its not ascii then use default 0x0C width
	mov r3, #0x0C ; Width
	strb r3, [r4,#2] ; Store current width
	add r5, #2
	
HandleCharacter_Continue:	
	LSL     R0, R0, #1		   	;ORIGINAL CODE
	ADD     R0, R0, R7	   		;ORIGINAL CODE
	LDRH    R3, [R0]		  	;ORIGINAL CODE
	LDRB    R1, [R4,#0x10]		;ORIGINAL CODE
	LDRB    R2, [R4,#0x12]		;ORIGINAL CODE

	
HandleCharacter_Exit:	
	bx lr

.pool

.align 4
WidthTable:
.incbin "bin/widthTable.bin"

.close

 ; make sure to leave an empty line at the end
