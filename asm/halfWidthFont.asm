 ; F-Zero Climax Character - Story Screen HWF by Normmatt

.gba				; Set the architecture to GBA
.open "rom/output.gba",0x08000000		; Open input.gba for output.
					; 0x08000000 will be used as the
					; header size
					
.macro adr,destReg,Address
here:
	.if (here & 2) != 0
		add destReg, r15, (Address-here)-2
	.else
		add destReg, r15, (Address-here)
	.endif
.endmacro

.thumb
.org 0x0805B854
	mov r1, #0x8        ; character width
	
.org 0x080430AE
	nop

.close

 ; make sure to leave an empty line at the end
