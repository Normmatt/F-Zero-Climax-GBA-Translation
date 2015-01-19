 ; F-Zero Climax Translation by Normmatt

.gba				; Set the architecture to GBA
.open "rom/output.gba",0x08000000	; Open input.gba for output.
						; 0x08000000 will be used as the
						; header size

.relativeinclude on

;Equs for Scripting and control codes
TextNL equ 0x0D
TextEnd equ .byte 0x0

.org 0x08CF7BAC
StoryEpisodeTitlePointerList:
	.word StoryEpisode1_Title
	.word StoryEpisode2_Title
	.word StoryEpisode3_Title
	.word StoryEpisode4_Title
	.word StoryEpisode5_Title
	.word StoryEpisode6_Title
	.word StoryEpisode7_Title
	.word StoryEpisode8_Title
	.word StoryEpisode9_Title
	.word StoryEpisode10_Title
	.word StoryEpisode11_Title

.org 0x08CF9D5C
StoryEpisodePointerList:
	.word StoryEpisode1PointerList
	.word StoryEpisode2PointerList
	.word StoryEpisode3PointerList
	.word StoryEpisode4PointerList
	.word StoryEpisode5PointerList
	.word StoryEpisode6PointerList
	.word StoryEpisode7PointerList
	.word StoryEpisode8PointerList
	.word StoryEpisode9PointerList
	.word StoryEpisode10PointerList
	.word StoryEpisode11PointerList

.org 0x08DD0000

EOF:
.byte 0x82,0x64,0x82,0x6E,0x82,0x65,0,0 ;EOF in SJIS

	.include episode_1/script.asm
	.include episode_2/script.asm
	.include episode_3/script.asm
	.include episode_4/script.asm
	.include episode_5/script.asm
	.include episode_6/script.asm
	.include episode_7/script.asm
	.include episode_8/script.asm
	.include episode_9/script.asm
	.include episode_10/script.asm
	.include episode_11/script.asm

.align 4
StoryEpisode1_Title:
	.incbin episode_1/title_lz.bin

.align 4
StoryEpisode2_Title:
	.incbin episode_2/title_lz.bin

.align 4
StoryEpisode3_Title:
	.incbin episode_3/title_lz.bin

.align 4
StoryEpisode4_Title:
	.incbin episode_4/title_lz.bin

.align 4
StoryEpisode5_Title:
	.incbin episode_5/title_lz.bin

.align 4
StoryEpisode6_Title:
	.incbin episode_6/title_lz.bin

.align 4
StoryEpisode7_Title:
	.incbin episode_7/title_lz.bin

.align 4
StoryEpisode8_Title:
	.incbin episode_8/title_lz.bin

.align 4
StoryEpisode9_Title:
	.incbin episode_9/title_lz.bin

.align 4
StoryEpisode10_Title:
	.incbin episode_10/title_lz.bin

.align 4
StoryEpisode11_Title:
	.incbin episode_11/title_lz.bin

.close
 ; make sure to leave an empty line at the end