#pragma once

typedef unsigned long DWORD;

template<class _ClassType>
_ClassType Call(DWORD addr)
{
	return ((_ClassType(*)(...))addr)();
}

template<class _ClassType, typename R3>
_ClassType Call(DWORD addr, R3 r3)
{
	return ((_ClassType(*)(...))addr)(r3);
}

template<class _ClassType, typename R3, typename R4>
_ClassType Call(DWORD addr, R3 r3, R4 r4)
{
	return ((_ClassType(*)(...))addr)(r3, r4);
}

template<class _ClassType, typename R3, typename R4, typename R5>
_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6>
_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7>
_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8>
_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9>
_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9, typename R10>
_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9, R10 r10)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9, r10);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9, typename R10,
	typename S1>
	_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9, R10 r10, S1 s1)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9, r10, s1);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9, typename R10,
	typename S1, typename S2>
	_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9, R10 r10, S1 s1, S2 s2)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9, r10, s1, s2);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9, typename R10,
	typename S1, typename S2, typename S3>
	_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9, R10 r10, S1 s1, S2 s2,
		S3 s3)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9, r10, s1, s2, s3);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9, typename R10,
	typename S1, typename S2, typename S3, typename S4>
	_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9, R10 r10, S1 s1, S2 s2,
		S3 s3, S4 s4)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9, r10, s1, s2, s3, s4);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9, typename R10,
	typename S1, typename S2, typename S3, typename S4, typename S5>
	_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9, R10 r10, S1 s1, S2 s2,
		S3 s3, S4 s4, S5 s5)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9, r10, s1, s2, s3, s4, s5);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9, typename R10,
	typename S1, typename S2, typename S3, typename S4, typename S5, typename S6>
	_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9, R10 r10, S1 s1, S2 s2,
		S3 s3, S4 s4, S5 s5, S6 s6)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9, r10, s1, s2, s3, s4, s5, s6);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9, typename R10,
	typename S1, typename S2, typename S3, typename S4, typename S5, typename S6, typename S7>
	_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9, R10 r10, S1 s1, S2 s2,
		S3 s3, S4 s4, S5 s5, S6 s6, S7 s7)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9, r10, s1, s2, s3, s4, s5, s6, s7);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9, typename R10,
	typename S1, typename S2, typename S3, typename S4, typename S5, typename S6, typename S7, typename S8>
	_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9, R10 r10, S1 s1, S2 s2,
		S3 s3, S4 s4, S5 s5, S6 s6, S7 s7, S8 s8)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9, r10, s1, s2, s3, s4, s5, s6, s7, s8);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9, typename R10,
	typename S1, typename S2, typename S3, typename S4, typename S5, typename S6, typename S7, typename S8,
	typename S9>
	_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9, R10 r10, S1 s1, S2 s2,
		S3 s3, S4 s4, S5 s5, S6 s6, S7 s7, S8 s8, S9 s9)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9, r10, s1, s2, s3, s4, s5, s6, s7, s8, s9);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9, typename R10,
	typename S1, typename S2, typename S3, typename S4, typename S5, typename S6, typename S7, typename S8,
	typename S9, typename S10>
	_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9, R10 r10, S1 s1, S2 s2,
		S3 s3, S4 s4, S5 s5, S6 s6, S7 s7, S8 s8, S9 s9, S10 s10)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9, r10, s1, s2, s3, s4, s5, s6, s7, s8, s9, s10);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9, typename R10,
	typename S1, typename S2, typename S3, typename S4, typename S5, typename S6, typename S7, typename S8,
	typename S9, typename S10, typename S11>
	_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9, R10 r10, S1 s1, S2 s2,
		S3 s3, S4 s4, S5 s5, S6 s6, S7 s7, S8 s8, S9 s9, S10 s10, S11 s11)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9, r10, s1, s2, s3, s4, s5, s6, s7, s8, s9, s10,
		s11);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9, typename R10,
	typename S1, typename S2, typename S3, typename S4, typename S5, typename S6, typename S7, typename S8,
	typename S9, typename S10, typename S11, typename S12>
	_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9, R10 r10, S1 s1, S2 s2,
		S3 s3, S4 s4, S5 s5, S6 s6, S7 s7, S8 s8, S9 s9, S10 s10, S11 s11, S12 s12)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9, r10, s1, s2, s3, s4, s5, s6, s7, s8, s9, s10,
		s11, s12);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9, typename R10,
	typename S1, typename S2, typename S3, typename S4, typename S5, typename S6, typename S7, typename S8,
	typename S9, typename S10, typename S11, typename S12, typename S13>
	_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9, R10 r10, S1 s1, S2 s2,
		S3 s3, S4 s4, S5 s5, S6 s6, S7 s7, S8 s8, S9 s9, S10 s10, S11 s11, S12 s12, S13 s13)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9, r10, s1, s2, s3, s4, s5, s6, s7, s8, s9, s10,
		s11, s12, s13);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9, typename R10,
	typename S1, typename S2, typename S3, typename S4, typename S5, typename S6, typename S7, typename S8,
	typename S9, typename S10, typename S11, typename S12, typename S13, typename S14>
	_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9, R10 r10, S1 s1, S2 s2,
		S3 s3, S4 s4, S5 s5, S6 s6, S7 s7, S8 s8, S9 s9, S10 s10, S11 s11, S12 s12, S13 s13, S14 s14)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9, r10, s1, s2, s3, s4, s5, s6, s7, s8, s9, s10,
		s11, s12, s13, s14);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9, typename R10,
	typename S1, typename S2, typename S3, typename S4, typename S5, typename S6, typename S7, typename S8,
	typename S9, typename S10, typename S11, typename S12, typename S13, typename S14, typename S15>
	_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9, R10 r10, S1 s1, S2 s2,
		S3 s3, S4 s4, S5 s5, S6 s6, S7 s7, S8 s8, S9 s9, S10 s10, S11 s11, S12 s12, S13 s13, S14 s14,
		S15 s15)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9, r10, s1, s2, s3, s4, s5, s6, s7, s8, s9, s10,
		s11, s12, s13, s14, s15);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9, typename R10,
	typename S1, typename S2, typename S3, typename S4, typename S5, typename S6, typename S7, typename S8,
	typename S9, typename S10, typename S11, typename S12, typename S13, typename S14, typename S15, typename S16>
	_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9, R10 r10, S1 s1, S2 s2,
		S3 s3, S4 s4, S5 s5, S6 s6, S7 s7, S8 s8, S9 s9, S10 s10, S11 s11, S12 s12, S13 s13, S14 s14,
		S15 s15, S16 s16)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9, r10, s1, s2, s3, s4, s5, s6, s7, s8, s9, s10,
		s11, s12, s13, s14, s15, s16);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9, typename R10,
	typename S1, typename S2, typename S3, typename S4, typename S5, typename S6, typename S7, typename S8,
	typename S9, typename S10, typename S11, typename S12, typename S13, typename S14, typename S15, typename S16,
	typename S17>
	_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9, R10 r10, S1 s1, S2 s2,
		S3 s3, S4 s4, S5 s5, S6 s6, S7 s7, S8 s8, S9 s9, S10 s10, S11 s11, S12 s12, S13 s13, S14 s14,
		S15 s15, S16 s16, S17 s17)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9, r10, s1, s2, s3, s4, s5, s6, s7, s8, s9, s10,
		s11, s12, s13, s14, s15, s16, s17);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9, typename R10,
	typename S1, typename S2, typename S3, typename S4, typename S5, typename S6, typename S7, typename S8,
	typename S9, typename S10, typename S11, typename S12, typename S13, typename S14, typename S15, typename S16,
	typename S17, typename S18>
	_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9, R10 r10, S1 s1, S2 s2,
		S3 s3, S4 s4, S5 s5, S6 s6, S7 s7, S8 s8, S9 s9, S10 s10, S11 s11, S12 s12, S13 s13, S14 s14,
		S15 s15, S16 s16, S17 s17, S18 s18)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9, r10, s1, s2, s3, s4, s5, s6, s7, s8, s9, s10,
		s11, s12, s13, s14, s15, s16, s17, s18);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9, typename R10,
	typename S1, typename S2, typename S3, typename S4, typename S5, typename S6, typename S7, typename S8,
	typename S9, typename S10, typename S11, typename S12, typename S13, typename S14, typename S15, typename S16,
	typename S17, typename S18, typename S19>
	_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9, R10 r10, S1 s1, S2 s2,
		S3 s3, S4 s4, S5 s5, S6 s6, S7 s7, S8 s8, S9 s9, S10 s10, S11 s11, S12 s12, S13 s13, S14 s14,
		S15 s15, S16 s16, S17 s17, S18 s18, S19 s19)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9, r10, s1, s2, s3, s4, s5, s6, s7, s8, s9, s10,
		s11, s12, s13, s14, s15, s16, s17, s18, s19);
}

template<class _ClassType, typename R3, typename R4, typename R5, typename R6, typename R7, typename R8, typename R9, typename R10,
	typename S1, typename S2, typename S3, typename S4, typename S5, typename S6, typename S7, typename S8,
	typename S9, typename S10, typename S11, typename S12, typename S13, typename S14, typename S15, typename S16,
	typename S17, typename S18, typename S19, typename S20>
	_ClassType Call(DWORD addr, R3 r3, R4 r4, R5 r5, R6 r6, R7 r7, R8 r8, R9 r9, R10 r10, S1 s1, S2 s2,
		S3 s3, S4 s4, S5 s5, S6 s6, S7 s7, S8 s8, S9 s9, S10 s10, S11 s11, S12 s12, S13 s13, S14 s14,
		S15 s15, S16 s16, S17 s17, S18 s18, S19 s19, S20 s20)
{
	return ((_ClassType(*)(...))addr)(r3, r4, r5, r6, r7, r8, r9, r10, s1, s2, s3, s4, s5, s6, s7, s8, s9, s10,
		s11, s12, s13, s14, s15, s16, s17, s18, s19, s20);
}