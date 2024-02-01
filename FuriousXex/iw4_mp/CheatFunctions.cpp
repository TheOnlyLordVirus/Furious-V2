#include "Cheats/CheatFunctions.h"
#include "Cheats/Math/CheatMath.h"
#include "Cheats/Calls.h"
#include "Utilities/Detours.h"
#include "Game/HudElems.h"

using namespace CheatMath;

void SetGodmode(bool value)
{


}

void SetInfAmmo(bool value)
{
	if (value)
		*reinterpret_cast<uint32_t*>(InfAmmoAddress) = InfAmmoEnableValue;
	else
		*reinterpret_cast<uint32_t*>(InfAmmoAddress) = InfAmmoDisableValue;
}

Detour <WeaponDef*> cWeapDefA;
struct gfxColor
{
	float r;
	uint32_t unk1;
	uint32_t unk2;
	uint32_t unk3;
	uint32_t unk4;
	float g;
	uint32_t unk5;
	uint32_t unk6;
	uint32_t unk7;
	uint32_t unk8;
	float b;
	uint32_t unk9;
	uint32_t unk10;
	uint32_t unk11;
	uint32_t unk12;
};
struct GfxSceneEntityMutableShaderData
{
	//uint32_t light;
	char pad1[0x1F];
	char shaderIndex;
};

typedef struct
{
	char name[0x20];//0x0
	char padding0[0x50]; //
	char LocalIp[0x04];//0x70
	char padding1[0x1A]; //
	char ip[0x04];//0x8E
	char port[0x02];//0x8
	char padding2[0xB0]; //0x94
}ipInfo_s;//0x140

typedef struct
{
	ipInfo_s info[18];
}ipInfo_ss;

#pragma endregion

#pragma endregion

#define btnMon (*(ButtonMonitor*)(btnAddr))
#define centity (*(centity_s**)(centityAddr))
#define centity2 (*(centity2_s**)(centityAddr))
#define cg (*(cg_s**)(cgAddr))
#define clientActive (*(clientActive_s**)(clientActiveAddr))

bool Dvar_GetBool(const char* dvarName)
{
	return Call<bool>(Dvar_GetBool1, dvarName);
}
static int(__cdecl* SL_GetString_)(const char* string, unsigned int user) = (int(__cdecl*)(const char* string, unsigned int user))0x82242320;

int SL_GetString(const char* string) {
	/*if (!scm(string, "tag_eye") || !_sys_strcmp(string, "j_helmet")) string = "j_head";*/
	return SL_GetString_(string, 0);
}

Vector3 AimTarget_GetTagPos(int client, const char* TagName, int x = 0, int y = 0, int z = 0)
{
	Vector3 Out(0, 0, 0);
	bool aimTarget = Call<bool>(aimTargetAddr, &centity[client], SL_GetString(TagName), &Out);
	if (aimTarget)
	{
		if (x != 0 || y != 0 || z != 0)
		{
			Out.x -= x;
			Out.y -= y;
			Out.z -= z;
		}
	}
	return Out;
}

void CG_LocationalTrace(trace_t* ptrace, const float* start, const float* end, int passEntityNum, int contentMask, bool checkRopes, int context)
{
	Call<void*>(CG_LocationalTraceAddr, ptrace, start, end, passEntityNum, contentMask, checkRopes, context);
}

bool isPositionVisible(Vector3 origin)
{
	trace_t trace;
	CG_LocationalTrace(&trace, (const float*)&cg->refDef.viewOrigin, (float*)&origin, cg->localClientNum, 0x803003, true, 0);
	return (trace.fraction >= 0.95f);
}

bool isClientAlive(int client)
{
	bool alive = (centity[client].alive != 0 && cg->clients[client].team != 0x03);
	if (alive)
		return true;
	else
		return false;
}

bool isSameTeam(int cl1, int cl2, bool ffa, bool all, bool team)
{
	if (ffa || all)
		return false;
	int ent1 = cg->clients[cl1].team;
	int ent2 = cg->clients[cl2].team;
	if (team)
	{
		if (ent1 != ent2)
			return true;
		else
			return false;
	}
	else
	{
		if (ent1 == ent2)
			return true;
		else
			return false;
	}
}

bool isGTffa()
{
	//char* gametype = Dvar_GetString("ui_gametype");
	//if (!_sys_strcmp(gametype, "dm") || !_sys_strcmp(gametype, "gun") || !_sys_strcmp(gametype, "oic") || !_sys_strcmp(gametype, "sas") || !_sys_strcmp(gametype, "shrp"))
	//	return true;
	//else
	return false;
}
const int tagLen = 10;
char* tags[tagLen] = {
	"j_head",
	"j_neck",
	"j_spine4",
	"pelvis",
	"j_elbow_ri",
	"j_elbow_le",
	"j_knee_ri",
	"j_knee_le",
	"j_ankle_ri",
	"j_ankle_le"
};
Vector2 finalAngles;

Vector3 dftOrg(0, 0, 0);
Vector3 userOrigin;
int userClient = -1;
bool overrideBoth = false;
bool overrideTeam = false;
Vector3 clientOrigin[18];
bool clientVis[18];
bool isActive[18];
bool isAlive[18];
bool sameTeam[18];
Vector3 targetBone[18];
float clientDis[18];


bool clientCheckAB(int client)
{
	if (userClient != client)
		if (isActive[client])
			if (isAlive[client])
				if (!sameTeam[client])
						return true;
	return false;
}
int getNearestClient()
{
	double nearestDistance = 1E+08f;
	int nearestClient = -1;

	for (int i = 0; i < 18; i++)
	{
		if (clientCheckAB(i))
		{
			bool check = true;
			check = clientVis[i];
			if (check)
			{
				float Distance = clientDis[i];
				if (Distance < nearestDistance)
				{
					nearestDistance = Distance;
					nearestClient = i;
				}
			}
		}
	}
	return nearestClient;
}

bool buttonPressed(int button)
{
	return (*(int*)(0x825DCAA4 + button - 4) == 1);
}

Vector3 Difference(Vector3 Target, Vector3 Entity)
{
	Vector3 Return;
	Return.x = Target.x - Entity.x;
	Return.y = Target.y - Entity.y;
	Return.z = Target.z - Entity.z;

	return Return;
}

Vector2 vectoangles(Vector3 Angles)
{
	float forward;
	float yaw, pitch;
	if (Angles.x == 0 && Angles.y == 0)
	{
		yaw = 0;
		if (Angles.z > 0) pitch = 90.00;
		else pitch = 270.00;
	}
	else
	{
		if (Angles.x != -1) yaw = (float)(atan2((double)Angles.y, (double)Angles.x) * 180.00 / PI);
		else if (Angles.y > 0) yaw = 90.00;
		else yaw = 270;
		if (yaw < 0) yaw += 360.00;

		forward = (float)sqrtf((double)(Angles.x * Angles.x + Angles.y * Angles.y));
		pitch = (float)(atan2((double)Angles.z, (double)forward) * 180.00 / PI);
	}
	Vector2 AnglesVector(-pitch, yaw);
	return AnglesVector;
}

float GetDistance(Vector3 c1, Vector3 c2)
{
	float dx = c2.x - c1.x;
	float dy = c2.y - c1.y;
	float dz = c2.z - c1.z;

	return sqrtf((float)((dx * dx) + (dy * dy) + (dz * dz)));
}


void loopFunc()
{
	inGame = Dvar_GetBool("cl_ingame");
	if (inGame)
	{
		userClient = cg->localClientNum;
		userOrigin = centity[userClient].origin;
		for (int i = 0; i < 18; i++)
		{
			isActive[i] = (bool)(centity[i].type == 1);
			if (isActive[i])
			{
					if (i != userClient)
						clientOrigin[i] = centity[i].origin;
					else
						clientOrigin[i] = dftOrg;

					isAlive[i] = isClientAlive(i);
				if (userClient != i)
				{
					clientDis[i] = GetDistance(clientOrigin[i], userOrigin);
					overrideBoth = false;
					sameTeam[i] = isSameTeam(userClient, i, isGTffa(), overrideBoth, overrideTeam);

					if (isAlive[i])
					{
						if (abStart)
						{

							targetBone[i] = AimTarget_GetTagPos(i, tags[0]);

						}
						Vector3 org = clientOrigin[i];
						org.z += 40;
						clientVis[i] = isPositionVisible(org);
					}
				}
			}
		}
	}
}
void SetAimbot(bool value)
{
	bool key = false;
	key = buttonPressed(button_lshldr);
	if (key)
	{
		int nearest = getNearestClient();

		if (nearest != -1)
		{
			Vector3 finPos = targetBone[nearest];
			Vector2 vec = vectoangles(Difference(finPos, cg->refDef.viewOrigin));
			finalAngles.x = (vec.x - clientActive->baseAngle.x);
			finalAngles.y = vec.y - clientActive->baseAngle.y;

			clientActive->viewAngle = finalAngles;
		}
	}
}

// Linearly interpolates between two colors
RGB lerp(const RGB& start, const RGB& end, float t) {
	RGB color;
	color.r = start.r + t * (end.r - start.r);
	color.g = start.g + t * (end.g - start.g);
	color.b = start.b + t * (end.b - start.b);
	return color;
}

void FloatFade(bool * toggle, float * output, float min, float max, float value) {
	if (*output >= max) {
		*toggle = true;
	}
	if (*output <= min) {
		*toggle = false;
	}


	if (*toggle) {
		*output -= value;
	}
	else {
		*output += value;
	}

}
void interpolateColors(float* resultColor, const float* color1, const float* color2, float weight) {
	for (int i = 0; i < 3; ++i) {
		resultColor[i] = (1.0f - weight) * color1[i] + weight * color2[i];
	}
}
float randomFloat() {
	return static_cast<float>(rand()) / static_cast<float>(RAND_MAX);
}

uint32_t floatColorToUInt(const float* color) {
	return static_cast<uint32_t>(color[0]) << 8 | static_cast<uint32_t>(color[1]) << 16 | static_cast<uint32_t>(color[2]) << 24 | static_cast<uint32_t>(color[3]);
}

#pragma region Disco
//#define COLOR_SIZE 11
//char colorIndex = 0;
//char colorSwapDelay = 0;
//RGB colors[COLOR_SIZE] = {
//  {1, .5, 0},
//  {1, 1, 0},
//  {.5, 1, 0},
//  {0, 1, 0},
//  {0, 1, .5},
//  {0, 1, 1},
//  {0, .5, 1},
//  {0, 0, 1},
//  {1, 0, 1},
//  {1, 0, .5},
//  {1, 0, 0}
//};

//if (colorSwapDelay != 1)
//{
//	colorSwapDelay++;

//	return;
//}

//colorSwapDelay = 0;

//if (colorIndex != COLOR_SIZE)
//	colorIndex++;
//else
//	colorIndex = 0;

//floatArray[0] = colors[colorIndex].r;
//floatArray[1] = colors[colorIndex].g;
//floatArray[2] = colors[colorIndex].b;
//floatArray[3] = 1.0f;
#pragma endregion

void __declspec(naked) Menu_PaintAllStub(int localClientNum, int a2)
{
	__asm
	{
		nop
		nop
		nop
		nop
		nop
		li r3, 7
		nop
		blr
	}
}


//void R_AddCmdDrawStretchPic_Detour(float* x, float* w, const float* col, materialx* shader)
//{
//	const float* saveColor = col;
//	color checkColor = (float*)&col;
//
//	if (strstr(shader->material, "popup"))
//	{
//		col = (float*)&color(255, 0, 0, 255);//saveColor[3] * 255.0f)
//	}
//
//	if (!strcmp(shader->material, "mw2_main_cloud_overlay"))// || _sys_strcmp(shader->material, "mw2_main_cloud_overlay")
//	{
//		checkColor.a = 0;
//		col = (float*)&checkColor;
//	}
//	if (!strcmp(shader->material, "mw2_main_background"))
//	{
//		x -= 34;
//		w += (34);
//	}
//	if (colorManager(shader->material, &checkColor))
//	{
//		checkColor.a = saveColor[3];
//		col = (float*)&checkColor;
//	}
//}
#pragma endregion