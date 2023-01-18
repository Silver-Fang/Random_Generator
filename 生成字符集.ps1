Get-Content "一级规范字.txt"|Set-Content -Path "一级规范字.u32" -Encoding "utf32" -NoNewline
Get-Content "二级规范字.txt"|Set-Content -Path "二级规范字.u32" -Encoding "utf32" -NoNewline
Get-Content "三级规范字.txt"|Set-Content -Path "三级规范字.u32" -Encoding "utf32" -NoNewline
function Write-UTF32
{
	param 
	(
		[Parameter(Mandatory)][UInt32]$Start,
		[Parameter(Mandatory)][UInt32]$End,
		[Parameter(Mandatory)][String]$Filename
	)
	$字节数=($End-$Start+1)*4
	$缓冲区=[Byte[]]::new($字节数)
	[Buffer]::BlockCopy([UInt32[]]($Start..$End),0,$缓冲区,0,$字节数)
	[IO.File]::WriteAllBytes($Filename,$缓冲区)
}
Write-UTF32 0x4e00 0x9fd5 "中日韩统一表意文字.u32"
Write-UTF32 0x3400 0x4db5 "扩展A.u32"
Write-UTF32 0x20000 0x2a6d6 "扩展B.u32"
Write-UTF32 0x2a700 0x2b734 "扩展C.u32"
Write-UTF32 0x2b740 0x2b81d "扩展D.u32"