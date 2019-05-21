rom=${1:-"rom/input.gba"}
out_rom=${2:-"rom/output.gba"}

# Checks that the rom is the correct one.
if [ ! -w "$rom" ] ; then
	echo "ROM is not present or is not writable"
	exit 1
else
	is_fzero_climax_jp=$(strings $rom | grep F-ZEROCLIMAXBFTJ01)
	if [ $is_fzero_climax_jp = "" ] ; then
		echo "ROM is not F-ZERO Climax"
		exit 1
	fi
fi

# Checks that the armips assembler is in the path.
armips=$(which armips)
if [ ! -x "$armips" ] ; then
   echo "ARMIPS compiler is missing, please install it and add it to your PATH."
   exit 1
fi

# Cleanup the output rom.
rm $out_rom

# Copy given gba file into the output rom read by the .asm files.
cp $rom $out_rom

# TODO : List all assembly files into something and loop on it.

armips -erroronwarning asm/misc.asm
armips -erroronwarning asm/variableWidthFont.asm
armips -erroronwarning asm/script/story/story.asm
armips -erroronwarning asm/script/profiles/profiles.asm
