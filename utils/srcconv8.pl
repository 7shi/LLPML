while(<>)
{
	s/Addr32\(Reg32/Addr32(REG32/g;
	s/Reg32\.EAX/Reg8.AL/g;
	s/Reg32\.EBX/Reg8.BL/g;
	s/Reg32\.ECX/Reg8.CL/g;
	s/Reg32\.EDX/Reg8.DL/g;
	s/Reg32\.ESP/Reg8.AH/g;
	s/Reg32\.EBP/Reg8.BH/g;
	s/REG32/Reg32/g;
	s/ eax/ al/g;
	s/ ebx/ bl/g;
	s/ ecx/ cl/g;
	s/ edx/ dl/g;
	s/ esp/ ah/g;
	s/ ebp/ bh/g;
	s/dword/byte/g;
	print $_;
}
