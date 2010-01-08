die "usage: perl conv.pl src asm" if $#ARGV != 1;
$src = shift;
$asm = shift;

open(FILE, $asm);
while (<FILE>)
{
	next if !/  (.+?) /;
	$code = $1;
	while ($code =~ s/([0-9A-F]{2})([0-9A-F]{2})/$1-$2/) {}
	push @codes, $code;
}
close(FILE);

open(FILE, $src);
while (<FILE>)
{
	if (/\, ".+"\)/)
	{
		$code = shift @codes;
		s/\, ".+"\)/, "$code")/;
	}
	print $_;
}
close(FILE);
