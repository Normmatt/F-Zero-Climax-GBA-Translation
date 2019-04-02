 ; F-Zero Climax Translation by Normmatt

.gba				; Set the architecture to GBA
.open "rom/output.gba",0x08000000	; Open input.gba for output.
						; 0x08000000 will be used as the
						; header size

.relativeinclude on

;Equs for Scripting and control codes
TextNL equ 0x0A
TextEnd equ .byte 0x0

.org 0x08CF9E54
ProfileInfoList:
	; Captain Falcon
	.byte 0x00 ; BloodType
	.byte 0x00 ; Age
	.byte 0x04 ; BirthMonth
	.byte 0x0A ; BirthDay
	.word Profile1_CharacterProfile
	.word Profile1_VehicleProfile

	; Dr. Stewart
	.byte 0x02 ; BloodType
	.byte 0x2A ; Age
	.byte 0x02 ; BirthMonth
	.byte 0x0E ; BirthDay
	.word Profile2_CharacterProfile
	.word Profile2_VehicleProfile

	; Pico
	.byte 0x03 ; BloodType
	.byte 0x7C ; Age
	.byte 0x05 ; BirthMonth
	.byte 0x12 ; BirthDay
	.word Profile3_CharacterProfile
	.word Profile3_VehicleProfile

	; Samurai Gorox
	.byte 0x03 ; BloodType
	.byte 0x2D ; Age
	.byte 0x04 ; BirthMonth
	.byte 0x16 ; BirthDay
	.word Profile4_CharacterProfile
	.word Profile4_VehicleProfile

	; Jody Summer
	.byte 0x01 ; BloodType
	.byte 0x00 ; Age
	.byte 0x02 ; BirthMonth
	.byte 0x15 ; BirthDay
	.word Profile5_CharacterProfile
	.word Profile5_VehicleProfile

	; Mighty Gazzele
	.byte 0x08 ; BloodType
	.byte 0x25 ; Age
	.byte 0x08 ; BirthMonth
	.byte 0x1B ; BirthDay
	.word Profile6_CharacterProfile
	.word Profile6_VehicleProfile

	; Baba
	.byte 0x02 ; BloodType
	.byte 0x13 ; Age
	.byte 0x06 ; BirthMonth
	.byte 0x1E ; BirthDay
	.word Profile7_CharacterProfile
	.word Profile7_VehicleProfile

	; Octoman
	.byte 0x03 ; BloodType
	.byte 0x00 ; Age
	.byte 0x08 ; BirthMonth
	.byte 0x08 ; BirthDay
	.word Profile8_CharacterProfile
	.word Profile8_VehicleProfile

	; Clash
	.byte 0x03 ; BloodType
	.byte 0x37 ; Age
	.byte 0x0C ; BirthMonth
	.byte 0x18 ; BirthDay
	.word Profile9_CharacterProfile
	.word Profile9_VehicleProfile

	; Ead
	.byte 0x08 ; BloodType
	.byte 0x00 ; Age
	.byte 0x0B ; BirthMonth
	.byte 0x10 ; BirthDay
	.word Profile10_CharacterProfile
	.word Profile10_VehicleProfile

	; Bio Rex
	.byte 0x08 ; BloodType
	.byte 0x09 ; Age
	.byte 0x01 ; BirthMonth
	.byte 0x0F ; BirthDay
	.word Profile11_CharacterProfile
	.word Profile11_VehicleProfile

	; Billy
	.byte 0x00 ; BloodType
	.byte 0x07 ; Age
	.byte 0x0A ; BirthMonth
	.byte 0x06 ; BirthDay
	.word Profile12_CharacterProfile
	.word Profile12_VehicleProfile

	; Silver Neelsen
	.byte 0x01 ; BloodType
	.byte 0x62 ; Age
	.byte 0x03 ; BirthMonth
	.byte 0x03 ; BirthDay
	.word Profile13_CharacterProfile
	.word Profile13_VehicleProfile

	; Gomar & Shioh
	.byte 0x07 ; BloodType
	.byte 0x00 ; Age
	.byte 0x05 ; BirthMonth
	.byte 0x19 ; BirthDay
	.word Profile14_CharacterProfile
	.word Profile14_VehicleProfile

	; John Tanaka
	.byte 0x01 ; BloodType
	.byte 0x1F ; Age
	.byte 0x05 ; BirthMonth
	.byte 0x07 ; BirthDay
	.word Profile15_CharacterProfile
	.word Profile15_VehicleProfile

	; Mrs Arrow
	.byte 0x03 ; BloodType
	.byte 0x1B ; Age
	.byte 0x08 ; BirthMonth
	.byte 0x1A ; BirthDay
	.word Profile16_CharacterProfile
	.word Profile16_VehicleProfile

	; Blood Falcon
	.byte 0x04 ; BloodType
	.byte 0x00 ; Age
	.byte 0x0A ; BirthMonth
	.byte 0x04 ; BirthDay
	.word Profile17_CharacterProfile
	.word Profile17_VehicleProfile

	; Jack Levin
	.byte 0x03 ; BloodType
	.byte 0x18 ; Age
	.byte 0x05 ; BirthMonth
	.byte 0x05 ; BirthDay
	.word Profile18_CharacterProfile
	.word Profile18_VehicleProfile

	; James McCloud
	.byte 0x00 ; BloodType
	.byte 0x20 ; Age
	.byte 0x07 ; BirthMonth
	.byte 0x0D ; BirthDay
	.word Profile19_CharacterProfile
	.word Profile19_VehicleProfile

	; Zoda
	.byte 0x07 ; BloodType
	.byte 0x2A ; Age
	.byte 0x05 ; BirthMonth
	.byte 0x13 ; BirthDay
	.word Profile20_CharacterProfile
	.word Profile20_VehicleProfile

	; Michael Chain
	.byte 0x01 ; BloodType
	.byte 0x27 ; Age
	.byte 0x0C ; BirthMonth
	.byte 0x10 ; BirthDay
	.word Profile21_CharacterProfile
	.word Profile21_VehicleProfile

	; Super Arrow
	.byte 0x00 ; BloodType
	.byte 0x23 ; Age
	.byte 0x09 ; BirthMonth
	.byte 0x18 ; BirthDay
	.word Profile22_CharacterProfile
	.word Profile22_VehicleProfile

	; Kate Alen
	.byte 0x03 ; BloodType
	.byte 0x19 ; Age
	.byte 0x0C ; BirthMonth
	.byte 0x18 ; BirthDay
	.word Profile23_CharacterProfile
	.word Profile23_VehicleProfile

	; Roger Buster
	.byte 0x00 ; BloodType
	.byte 0x29 ; Age
	.byte 0x05 ; BirthMonth
	.byte 0x0A ; BirthDay
	.word Profile24_CharacterProfile
	.word Profile24_VehicleProfile

	; Leon
	.byte 0x00 ; BloodType
	.byte 0x10 ; Age
	.byte 0x0A ; BirthMonth
	.byte 0x15 ; BirthDay
	.word Profile25_CharacterProfile
	.word Profile25_VehicleProfile

	; Draq
	.byte 0x08 ; BloodType
	.byte 0x89 ; Age
	.byte 0x03 ; BirthMonth
	.byte 0x1E ; BirthDay
	.word Profile26_CharacterProfile
	.word Profile26_VehicleProfile

	; Beastman
	.byte 0x08 ; BloodType
	.byte 0x1E ; Age
	.byte 0x00 ; BirthMonth
	.byte 0x00 ; BirthDay
	.word Profile27_CharacterProfile
	.word Profile27_VehicleProfile

	; Antonio Guster
	.byte 0x01 ; BloodType
	.byte 0x24 ; Age
	.byte 0x09 ; BirthMonth
	.byte 0x09 ; BirthDay
	.word Profile28_CharacterProfile
	.word Profile28_VehicleProfile

	; Black Shadow
	.byte 0x08 ; BloodType
	.byte 0x00 ; Age
	.byte 0x08 ; BirthMonth
	.byte 0x0F ; BirthDay
	.word Profile29_CharacterProfile
	.word Profile29_VehicleProfile

	; The Skull
	.byte 0x05 ; BloodType
	.byte 0xF1 ; Age
	.byte 0x01 ; BirthMonth
	.byte 0x17 ; BirthDay
	.word Profile30_CharacterProfile
	.word Profile30_VehicleProfile

	; Ryu Suzaku
	.byte 0x00 ; BloodType
	.byte 0x17 ; Age
	.byte 0x07 ; BirthMonth
	.byte 0x1C ; BirthDay
	.word Profile31_CharacterProfile
	.word Profile31_VehicleProfile

	; Lisa Brilliant
	.byte 0x00 ; BloodType
	.byte 0x00 ; Age
	.byte 0x08 ; BirthMonth
	.byte 0x02 ; BirthDay
	.word Profile32_CharacterProfile
	.word Profile32_VehicleProfile

	; Lucy Liberty
	.byte 0x02 ; BloodType
	.byte 0x14 ; Age
	.byte 0x09 ; BirthMonth
	.byte 0x12 ; BirthDay
	.word Profile33_CharacterProfile
	.word Profile33_VehicleProfile

	; Miss Killer
	.byte 0x00 ; BloodType
	.byte 0x14 ; Age
	.byte 0x01 ; BirthMonth
	.byte 0x03 ; BirthDay
	.word Profile34_CharacterProfile
	.word Profile34_VehicleProfile

	; Dark Soldier
	.byte 0x08 ; BloodType
	.byte 0x00 ; Age
	.byte 0x00 ; BirthMonth
	.byte 0x00 ; BirthDay
	.word Profile35_CharacterProfile
	.word Profile35_VehicleProfile

	; Berserker
	.byte 0x08 ; BloodType
	.byte 0x00 ; Age
	.byte 0x00 ; BirthMonth
	.byte 0x00 ; BirthDay
	.word Profile36_CharacterProfile
	.word Profile36_VehicleProfile

	; Clank Hughes
	.byte 0x03 ; BloodType
	.byte 0x0B ; Age
	.byte 0x07 ; BirthMonth
	.byte 0x1C ; BirthDay
	.word Profile37_CharacterProfile
	.word Profile37_VehicleProfile

	; (unused)
	.byte 0x00 ; BloodType
	.byte 0x00 ; Age
	.byte 0x00 ; BirthMonth
	.byte 0x00 ; BirthDay
	.word Profile38_CharacterProfile
	.word Profile38_VehicleProfile

	; Hyper Zoda
	.byte 0x07 ; BloodType
	.byte 0x2A ; Age
	.byte 0x05 ; BirthMonth
	.byte 0x13 ; BirthDay
	.word Profile39_CharacterProfile
	.word Profile39_VehicleProfile



.org 0x08DE0000

	.include "profile_1/script.asm"
	.include "profile_2/script.asm"
	.include "profile_3/script.asm"
	.include "profile_4/script.asm"
	.include "profile_5/script.asm"
	.include "profile_6/script.asm"
	.include "profile_7/script.asm"
	.include "profile_8/script.asm"
	.include "profile_9/script.asm"
	.include "profile_10/script.asm"
	.include "profile_11/script.asm"
	.include "profile_12/script.asm"
	.include "profile_13/script.asm"
	.include "profile_14/script.asm"
	.include "profile_15/script.asm"
	.include "profile_16/script.asm"
	.include "profile_17/script.asm"
	.include "profile_18/script.asm"
	.include "profile_19/script.asm"
	.include "profile_20/script.asm"
	.include "profile_21/script.asm"
	.include "profile_22/script.asm"
	.include "profile_23/script.asm"
	.include "profile_24/script.asm"
	.include "profile_25/script.asm"
	.include "profile_26/script.asm"
	.include "profile_27/script.asm"
	.include "profile_28/script.asm"
	.include "profile_29/script.asm"
	.include "profile_30/script.asm"
	.include "profile_31/script.asm"
	.include "profile_32/script.asm"
	.include "profile_33/script.asm"
	.include "profile_34/script.asm"
	.include "profile_35/script.asm"
	.include "profile_36/script.asm"
	.include "profile_37/script.asm"
	.include "profile_38/script.asm"
	.include "profile_39/script.asm"

.close
 ; make sure to leave an empty line at the end