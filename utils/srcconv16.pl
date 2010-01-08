while(<>)
{
	s/Addr32\(Reg32/Addr32(REG32/g;
	s/Reg32\.E/Reg16./g;
	s/REG32/Reg32/g;
	s/ e/ /g;
	s/dword/word/g;
	print $_;
}
