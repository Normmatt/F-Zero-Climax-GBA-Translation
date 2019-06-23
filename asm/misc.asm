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
.incbin "asm/bin/font.bin"

.org 0x08DCAB00
Controls_LZ:
	.incbin "asm/bin/controls_eng_lz.bin"

.close

 ; make sure to leave an empty line at the end
