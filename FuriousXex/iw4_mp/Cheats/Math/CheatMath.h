#pragma once

#include <stdio.h>
#include <math.h>
#define PI (3.1415926535897931)

namespace CheatMath
{
	float abs(float a);
	float pow(float num, int power);
	float root(float num, int nroot);
	float sin(float deg);
	float cos(float AnglesDeg);
	float tan(float AnglesDeg);
	double atan(double x, int n);
	double atan(double y, double x);
	float DegreesToRadians(float a);
	float RadiansToDegrees(float a);

	typedef struct Vector3
	{
		float x, y, z;
		Vector3()
		{
			x = y = z = 0;
		}
		Vector3(float x, float y, float z)
		{
			this->x = x; this->y = y; this->z = z;
		}
		const Vector3& operator-(void) const {
			return Vector3(-x, -y, -z);
		}
		const bool operator==(const Vector3& v) const {
			return x == v.x && y == v.y && z == v.z;
		}
		const bool operator!=(const Vector3& v) const {
			return !(*this == v);
		}
		const Vector3& operator+(const Vector3& v) const {
			return Vector3(x + v.x, y + v.y, z + v.z);
		}
		const Vector3& operator-(const Vector3& v) const {
			return Vector3(x - v.x, y - v.y, z - v.z);
		}
		const Vector3& operator*(float fl) const {
			return Vector3(x * fl, y * fl, z * fl);
		}
		const Vector3& operator/(float fl) const {
			return Vector3(x / fl, y / fl, z / fl);
		}
		const float Length(void) const {
			return (float)sqrt(x * x + y * y + z * z);
		}
		float Vector3::getLength() const {
			return sqrt((x * x) + (y * y) + (z * z));
		}
		const Vector3 Normalize(void) const
		{
			float flLen = Length();
			if (flLen == 0) return Vector3(0, 0, 1); // ????
			flLen = 1 / flLen;
			return Vector3(x * flLen, y * flLen, z * flLen);
		}

		float Distance(Vector3 const& Vector)
		{
			return sqrt(DistanceEx(Vector));
		}
		float DistanceEx(Vector3 const& Vector)
		{
			float _x = this->x - Vector.x, _y = this->y - Vector.y, _z = this->z - Vector.z;
			return ((_x * _x) + (_y * _y) + (_z * _z));
		}
		float DotProduct(Vector3 const& Vector)
		{
			return (this->x * Vector.x) + (this->y * Vector.y) + (this->z * Vector.z);
		}
		const Vector3& RoundHalfUp()
		{
			return Vector3(floor(this->x + 0.5), floor(this->y + 0.5), floor(this->z + 0.5));
		}
		const Vector3& RoundHalfDown()
		{
			return Vector3(floor(this->x + 0.5), floor(this->y + 0.5), floor(this->z + 0.5));
		}
	} Vector3, *PVector3;

	typedef struct Vector2
	{
		float x, y;
		Vector2()
		{
			this->x = this->y = 0;
		}

		Vector2(float x, float y)
		{
			this->x = x;
			this->y = y;
		}

		bool operator==(Vector2& Vec)
		{
			return (this->x == Vec.x && this->y == Vec.y);
		}
		const Vector2& operator- (Vector2 const& Vector)
		{
			return Vector2(this->x - Vector.x, this->y - Vector.y);
		}
		const Vector2& operator+ (Vector2 const& Vector)
		{
			return Vector2(this->x + Vector.x, this->y + Vector.y);
		}
	}Vector2, *PVector2;
}
