 ; F-Zero Climax Translation by Normmatt

.gba				; Set the architecture to GBA
.open "rom/output.gba",0x08000000		; Open input.gba for output.
					; 0x08000000 will be used as the
					; header size
.thumb

; Move all ascii font characters up by one pixel				
.org 0x0807AD40
.word Controls_LZ

; Move all ascii font characters up by one pixel				
.org 0x0837CBCC
.incbin asm/bin/font.bin
				
; Fix lowercase a in the font				
;.org 0x0837EC4C
;.incbin asm/bin/lowercase_a.bin

;.org 0x0805B6C0
;.area 0x0805B6D4 - 0x0805B6C0
;	mov r0, #11
;	add r0,r15
;	mov r14, r0
;    ldr r0, =ReadableText+1    ; r0 is best variable to use for jump
;    bx r0
;.pool
;.endarea
;

;This only modifies story 1
;TODO: Workout the best way to do this on all story's
.org 0x08DCAB00
;ReadableText:
;	;LDRB    R0, [R6,#0x19]
;	;LSLS    R0, R0, #2
;	;ADDS    R0, R0, R2
;	;LDR     R0, [R0]
;	;BL      LZ77UncompVRAM
;	;LDR     R1, =off_8CF7934
;	;LDRB    R0, [R6,#0x19]
;	;LSLS    R0, R0, #2
;	;ADDS    R0, R0, R1
;	
;	ldrb r0, [r6,#0x19]
;	lsl r0, r0, #2
;	add r0, r0, r2
;	ldr r0, [r0]
;	swi 0x12
;	
;	mov r0, #0x06
;	lsl r0, r0, #24 ;r0 = 0x06000000
;	
;	add r0, #0x94
;	
;	ldr r1, [r0]
;	str r1, [r0,#4]
;	str r1, [r0,#8]
;	str r1, [r0,#12]
;	str r1, [r0,#16]
;	str r1, [r0,#20]
;	str r1, [r0,#24]
;	
;	mov r1, #0x0D
;	lsl r1, #7 ;r1 = 0x680
;	
;	add r0, r0, r1
;	
;	ldr r1, [r0]
;	str r1, [r0,#4]
;	str r1, [r0,#8]
;	str r1, [r0,#12]
;	str r1, [r0,#16]
;	str r1, [r0,#20]
;	str r1, [r0,#24]
;	
;	ldr r1, =0x08cf7934
;	ldrb r0, [r6,#0x19]
;	
;	bx lr
;	
;.pool

Controls_LZ:
	.incbin asm/bin/controls_eng_lz.bin

.close

 ; make sure to leave an empty line at the end
