#pragma once

#include "../Cheats/Math/CheatMath.h"

using namespace CheatMath;
struct UiContext
{
int x;    // 0x0000
int y;    // 0x0004
int frameTime;  // 0x0008
int realTime;  // 0x000C
char padding[0x10]; // 0x0010
int width;   // 0x001C
int height;   // 0x0020
float aspect;  // 0x0024
float fps;   // 0x0028
};



//typedef struct
//{
//	char pad1[0x88];//0
//	int score; //0x88
//	int kills; //0x8C
//	char pad2[0x4];//0x90
//	int deaths; //0x94
//	char pad3[0x14];//0x98
//	int assists;//0xAC
//	char pad4[0x10];//0xB0
//	int killstreak;//0xC0
//	char pad5[0x744];//0xC4
//}score_s;//0x808

typedef struct
{
	int client; //0x0
	int score; //0x4
	int ping; //0x8
	int deaths; //0xC
	int assists; //0x10
	int team; //0x14
	char _0x0018[0x10];
}score_s;// size 0x28

typedef struct
{
	char pad0[0x69DAC];
	score_s score[18];
}clientInfoScore;

//typedef struct
//{
//	char name[0x20];//0x0 | 0x3783042C
//	int team;//0x20 | 0x3783044C
//	char padding0[0x5C]; //0x24
//	int aliveCheck;//0x80 | 0x378304AC
//	char padding1[0x434]; //0x84
//	Vector2 view;//0x4B8 | 0x378308E4
//	char padding2[0xB8]; //0x4C0
//	int stance;//0x578 | 0x378309A4
//	char padding3[0x4]; //0x57C
//	int checkFire;//0x580 | 0x378309AC
//	char padding4[0x284];
//}clientInfo_s;//0x808

typedef struct
{
	int valid;						//0x00
	char padding00[0x8];			//0x04
	char name[32];					//0x0C
	int team;						//0x2C
	char padding01[0x4];			//0x30
	int rank;						//0x34
	int prestige;					//0x38
	char padding02[0xC];			//0x3C
	unsigned int Perk[2];			//0x48
	char padding03[0x4];			//0x4C
	int score;						//0x54
	char model[0x40];				//0x58
	char headModel[0x40];			//0x98
	char weaponModel[0x40];			//0xD8
	char weaponModel2[0x40];		//0x117
	char padding04[0x2C0];			//0x158
	Vector2 view;					//0x418
	char _0x0424[0x7C];				//0x424
	int stance;						//0x49C
	char padddds[0x1C];				//0x4A0
	int zooming;					//0x4BC	
	char padding06[0x3C];			//0x4C0
	int aliveCheck;					//0x4FC
	char padds[0x18];				//0x500
	int weapon;						//0x518
	char padding07[0x28];			//0x51C

}clientInfo_s;

//typedef struct
//{
//	int width;      //0x8
//	int height;      //0xC
//	char padding01[0x14];   //0x10
//	Vector3 fov_v;     //0x24
//	char padding02[0x4];   //0x30
//	float fov;      //0x34
//	Vector3 viewOrigin;    //0x38
//	char padding03[0x4];   //0x44
//	Vector3 viewAxis[3];   //0x48
//} refDef_s;// 0x6C

typedef struct
{
	int x;							//0x00
	int y;							//0x04
	int width;						//0x08
	int height;						//0x0C
	Vector2 fov_v;					//0x10
	Vector3 viewOrigin;				//0x18
	Vector3 viewAxis[3];			//0x24
	char _0x0048[0x18];
	float fov;//0x60
	//0x64
}refDef_s;

//typedef struct
//{
//	int localClientNum;//0x0 | 0x377C6680
//	char padding0[0x80]; //0x4
//	int ping;//0x84 | 0x377C6704
//	char padding1[0x20]; //0x88
//	int playerFlag;//0xA8 | 0x377C6728
//	char padding2[0x1B7];//0xAC
//	char actionIndex;//0x263 | 0x377c68e0
//	char padding3[0x0C];
//	float aimRadio;//0x270 | 0x377c68f0
//	char padding4[0x54];
//	int health;//0x2C8 | 0x377C6948
//	char padding5[0x4]; //0x2CC
//	int healthMax;//0x2D0 | 0x377C6950
//	char padding6[0x1B8]; //0x2D4
//	int secClip;//0x48C | 0x377C6B0C
//	int primClip;//0x490 | 0x377C6B10
//	char padding7[0x34]; //0x494
//	int secAmmo;//0x4C8 | 0x377C6B48
//	int primAmmo;//0x4CC | 0x377C6B4C
//	int lethalAmmo;//0x4D0 | 0x377C6B50
//	int tactialAmmo;//0x4D4 | 0x377C6B54
//	char padding8[0x4C]; //0x4D8
//	int pickUpItem;//0x524 | 0x377C6BA4
//	char padding9[0x4D670]; //0x528
//	refDef_s refDef;//0x4DB98 | 0x37814218
//	char padding10[0x1C1B0]; //0x4DB98
//	clientInfo_s clients[18];//0x69DAC | 0x3783042C
//	char padding11[0xE054];//0x72E3C
//	float spreadMulti; //0x80E90
//} cgg_s;
//0x270 | 0x377c68f0

typedef struct
{
	int servertime;					//0x00
	char padding00[0x100];			//0x04
	int localClientNum;				//0x104
	char padding01[0x4];			//0x108
	Vector3 viewAngle;				//0x10C
	char padding03[0x38];			//0x118
	int health;						//0x150
	char padds[0x4];//0x154
	int healthMax;//0x158
	char paddingg04[0x9C];			//0x15C 
	int actionIndex;//0x1F8 | 0xAD3A81F8
	char paddingg0[0xC4]; //0x4
	int aimRadio;//0x2C0 | 0xAD3A82C0
	float spreadMulti;//+0x4 | 0xAD3A82C4
	char padding0[0x14]; //+0x8
	int secClip;//0x1C | 0xAD3A82DC
	char padding1[0xC]; //0x20
	int primeClip;//0x2C | 0xAD3A82EC
	char padding2[0x64]; //0x30
	int secAmmo;//0x94 | 0xAD3A8354
	char padding3[0x8]; //0x98
	int lethalAmmo;//0xA0 | 0xAD3A8360
	char padding4[0x8]; //0xA4
	int primAmmo;//0xAC | 0xAD3A836C
	char padding5[0x8]; //0xB0
	int tactialAmmo;//0xB8 | 0xAD3A8378
	char paddingg06[0x6A8AC];		//0x37C
	refDef_s refDef;				//0x6AC28 | 0xAD412C28
	char _0x6AC8C[0x84E4];
	score_s score[18];				//0x73170
	char _0x73440[0x90];
	int weapon;						//0x734D0  
	char padding08[0x839AC];		//0x734D4
	clientInfo_s clients[18];	//0xF6E80 | 0xAD49EE80
} cg_s;

//typedef struct {
//	int start;//0x0 | 0x37910610
//	char padding0[0x28]; //0x4
//	Vector3 origin;//0x2C | 0x3791063C
//	char padding1[0x178]; //0x38
//	int primWeap;//0x1B0
//	int secWeap;//0x1B4
//	char padding2[0x18];//0x1B8
//	int clientNum;//0x1D0 | 0x379107E0
//	char padding3[0xD4]; //0x1D4
//	short type;//0x2A8 | 0x379108B8
//	char padding4[0xA]; //0x2AA
//	int weapon;//0x2B4 | 0x379108C4
//	char padding5[0xB4]; //0x2B8
//	char state;//0x36C | 0x3791097C
//	char padding6[0x7];//0x36D
//}centity_s;//0x374

typedef struct
{
	char _0x0000[0x2];				//0x0000
	char alive;						//0x0002
	char _0x0003[0x15];				//0x0003
	Vector3 origin;					//0x0018
	char _0x0024[0x3C];				//0x0024
	int flags;						//0x0060
	char _0x0064[0x14];				//0x0064
	Vector3 newOrigin;				//0x0078
	char _0x0084[0x4C];				//0x0084
	int handle;						//0x00D0
	int type;						//0x00D4
	char _0x00D8[0x1C];				//0x00D8
	Vector3 oldOrigin;				//0x00F4
	char _0x0100[0x58];				//0x0100
	int pickupID;					//0x0158
	char _0x015C[0x40];				//0x015C
	short weapon;					//0x019C
	char _0x019E[8];				//0x019E
	short oldWeapon;				//0x01A6
	char _0x01B0[0x50];				//0x01A8
}centity_s;			//0x01F8

enum stance_t
{
	STANCE_STANDING = 0x0,
	STANCE_CROUCH = 0x1,
	STANCE_PRONE = 0x2,
};

enum entityType_t
{
	ET_General = 0,
	ET_Player = 1,
	ET_Corpse = 2,
	ET_Item = 3,
	ET_Missile = 4,
	ET_Invisible_entity = 5,
	ET_Scriptmover = 6,
	ET_Sound_blend = 7,
	ET_Fx = 8,
	ET_Loop_FX = 9,
	ET_Primary_Light = 10,
	ET_Turret = 11,
	ET_Helicopter = 12,
	ET_Plane = 13,
	ET_Vehicle = 14,
	ET_Vehicle_corpse = 15,
	ET_Actor = 16,
	ET_Actor_spawner = 17,
	ET_Actor_corpse = 18,
	ET_Streamer_Hint = 19,
};

typedef struct
{
	int time;						// 0x00-0x04
	int buttons;					// 0x04-0x08
	int viewAngles[3];				// 0x08-0x14
	short weapon;					// 0x14-0x16
	short offHandIndex;				// 0x16-0x18
	char Buf[0x02];					// 0x18-0x1A
	char fDir;						// 0x1A-0x1B
	char rDir;						// 0x1B-0x1C
	char Buf2[0x0C];				// 0x1C-0x28
}usercmd_s;

//typedef struct
//{
//	char padding00[0x44];          // 0x00
//	bool ADS;       // 0x44
//	char padding01[0xC3];    // 0x45
//	Vector3 baseAngle;     // 0x108
//	char padding02[0x2B80];    // 0x114
//	Vector2 viewAngle;     // 0x2C94
//	char padding03[0x40014];   // 0x2C9C
//	usercmd_s UserCmd[128];    // 0x42CB0
//	int CurrentCmdNumber;    // 0x44AB0
//}clientActive_s;

typedef struct
{
	bool ADS;						//0x0000
	char padding00[0xDF];			//0x0001
	Vector3 baseAngle;				//0x00E0
	char padding01[0x31B7];			//0x00EC
	char mapName[0x40];				//0x32A3
	char padding02[0x21];			//0x32E3
	int stance;						//0x3304
	char padding03[0x28];			//0x3308
	Vector3 origin;					//0x3330
	char padding04[0x1C];			//0x333C
	Vector2 viewAngle;				//0x3358
	char padding05[0x04];			//0x335C
	usercmd_s UserCmd[128];			//0x3364
	int CurrentCmdNumber;			//0x4764
}clientActive_s;


struct trace_t
{
	float fraction;					//0x0000
	Vector3 normal;					//0x0004
	int surfaceFlags;				//0x0010
	int contents;					//0x0014
	const char* material;			//0x0018
	int hitType;					//0x001C
	unsigned short hitId;			//0x0020
	unsigned short modelIndex;		//0x0022
	unsigned short partName;		//0x0024
	unsigned short partGroup;		//0x0026
	bool allsolid;					//0x0028 allsolid
	bool startsolid;				//0x0029 startsolid
	char _0x002B[0x02];				//0x002B
}; // 0x002C

struct BulletTraceResults
{
	trace_t trace;				// 0x0000
	int* hitEnt;				// 0x002C
	Vector3 hitPos;				// 0x0030
	int ignoreHitEnt;			// 0x003C
	int depthSurfaceType;		// 0x0040
	char _padding[0x04];		// 0x0044
}; // 0x48

struct BulletFireParams
{
	int weaponEntIndex;					// 0000
	int ignoreEntIndex;				// 0004
	float damageMultiplier;			// 0008
	int methodOfDeath;				// 000C
	Vector3 origStart;				// 0010
	Vector3 start;					// 0001C
	Vector3 end;					// 00028
	Vector3 dir;					// 00034
									// 00040
};

enum weapType_t
{
	WEAPTYPE_BULLET,
	WEAPTYPE_GRENADE,
	WEAPTYPE_PROJECTILE,
	WEAPTYPE_BINOCULARS
};

enum weapClass_t
{
	WEAPCLASS_RIFLE,
	WEAPCLASS_SNIPER,
	WEAPCLASS_MG,
	WEAPCLASS_SMG,
	WEAPCLASS_SPREAD,
	WEAPCLASS_PISTOL,
	WEAPCLASS_GRENADE,
	WEAPCLASS_ROCKETLAUNCHER,
	WEAPCLASS_TURRET,
	WEAPCLASS_THROWINGKNIFE,
	WEAPONCLASS_NON_PLAYER,
	WEAPCLASS_ITEM
};

enum PenetrateType
{
	PENETRATE_TYPE_NONE = 0x0,
	PENETRATE_TYPE_SMALL = 0x1,
	PENETRATE_TYPE_MEDIUM = 0x2,
	PENETRATE_TYPE_LARGE = 0x3,
	PENETRATE_TYPE_COUNT = 0x4,
};

struct WeaponDef {
	char padding[0x2C]; // 0x0
	weapType_t weaponType; // 0x2C
	weapClass_t weaponClass; // 0x30
	PenetrateType penetrateType; // 0x34
	char padding2[0x624]; // 0x38
	bool rifleBullet; // 0x65C
	char padding3[4]; // 0x65D
	bool bBulletImpactExplode; // 0x661
};

struct class_helper {
	unsigned int at(int num) {
		return (unsigned int)(this) + num;
	}

	template<typename T> T get(int num) {
		return *reinterpret_cast<T*>((unsigned int)(this) + num);
	}

	template<typename T> void set(int num, T value) {
		*reinterpret_cast<T*>((unsigned int)(this) + num) = value;
	}

	template<typename T> void inc(int num, T value) {
		*reinterpret_cast<T*>((unsigned int)(this) + num) += value;
	}
};

struct GfxDepthOfField {
	float viewModelStart;
	float viewModelEnd;
	float nearStart;
	float nearEnd;
	float farStart;
	float farEnd;
	float nearBlur;
	float farBlur;
};

struct GfxFilm {
	bool enabled;
	float brightness;
	float contrast;
	float desaturation;
	float desaturationDark;
	bool invert;
	float tintDark[3];
	float tintMedium[3];
	float tintLight[3];
};

struct GfxGlow {
	bool enabled;
	float bloomCutoff;
	float bloomDesaturation;
	float bloomIntensity;
	float radius;
};

struct GfxLightScale {
	float diffuseScale;
	float specularScale;
};

struct __declspec(align(2)) Stage {
	const char *name;
	float origin[3];
	unsigned __int16 triggerIndex;
	unsigned __int8 sunPrimaryLightIndex;
};


struct __declspec(align(4)) GfxStageInfo {
	Stage activeStage;
	bool activeStageValid;
};

struct __declspec(align(4)) GfxCompositeFx {
	GfxFilm film;
	float distortionScale[3];
	float blurRadius;
	float distortionMagnitude;
	float frameRate;
	int lastUpdate;
	int frame;
	int startMSec;
	int currentTime;
	int duration;
	bool enabled;
	bool scriptEnabled;
};

struct GfxViewport {
	int x;
	int y;
	int width;
	int height;
};
typedef struct color
{
	float r, g, b, a;
	color() { r = g = b = a = a; }
	color(float r, float g, float b, float a) { this->r = r / 255.0f, this->g = g / 255.0f, this->b = b / 255.0f, this->a = a / 255.0f; }
	color(float r, float g, float b) { this->r = r / 255.0f, this->g = g / 255.0f, this->b = b / 255.0f, 1; }
	color(float* rgb) { this->r = rgb[0] / 255.0f, this->g = rgb[1] / 255.0f, this->b = rgb[2] / 255.0f, this->a = rgb[3] / 255.0f; }
	color(float* rgb, float a) { this->r = rgb[0] / 255.0f, this->g = rgb[1] / 255.0f, this->b = rgb[2] / 255.0f, this->a = a / 255.0f; }

}color;
typedef struct colorRGB
{
	float r, g, b;
	colorRGB() { r = g = b = b; }
	colorRGB(float r, float g, float b) { this->r = r / 255.0f, this->g = g / 255.0f, this->b = b / 255.0f; }
	colorRGB(float* rgb) { this->r = rgb[0] / 255.0f, this->g = rgb[1] / 255.0f, this->b = rgb[2] / 255.0f; }

}colorRGB;

struct GfxLight {
	unsigned __int8 type;
	unsigned __int8 canUseShadowMap;
	unsigned __int8 unused[2];
	colorRGB Color;
	float dir[3];
	float origin[3];
	float radius;
	float cosHalfFovOuter;
	float cosHalfFovInner;
	int exponent;
	unsigned int spotShadowIndex;
	void *def; //GfxLightDef
};

struct GfxSceneParms {
	int localClientNum;
	float blurRadius;
	bool playerTeleported;
	bool viewModelHasDistortion;
	unsigned __int8 forceSunShadowsGenerate;
	bool halfResParticles;
	GfxDepthOfField dof;
	GfxFilm film;
	GfxGlow glow;
	GfxLightScale charPrimaryLightScale;
	GfxStageInfo stageInfo;
	GfxCompositeFx waterSheetingFx;
	GfxViewport sceneViewport;
	GfxViewport displayViewport;
	GfxViewport scissorViewport;
	GfxLight primaryLights[248];
};

struct GfxMatrix {
	float m[4][4];
};

struct GfxCamera {
	float origin[3];
	float axis[3][3];
	float subWindowMins[2];
	float subWindowMaxs[2];
	float tanHalfFovX;
	float tanHalfFovY;
	float zNear;
	float depthHackNearClip;
};

struct GfxViewParms {
	GfxMatrix viewMatrix;
	GfxMatrix projectionMatrix;
	GfxMatrix viewProjectionMatrix;
	GfxMatrix inverseViewProjectionMatrix;
	GfxCamera camera;
};


struct GfxFog {
	int startTime;
	int finishTime;
	unsigned int color;
	float fogStart;
	float density;
	float fogMaxOpacity;
	bool sunFogEnabled;
	unsigned int sunColor;
	float sunDir[3];
	float sunBeginFadeAngle;
	float sunEndFadeAngle;
	float sunFogScale;
};

struct GfxVertexBufferState {
	volatile int used;
	int total;
	char buffer[32];
	unsigned __int8 *verts;
};


struct GfxMeshData {
	unsigned int indexCount;
	unsigned int totalIndexCount;
	char ib[32];
	unsigned __int16 *indices;
	GfxVertexBufferState vb;
	unsigned int vertSize;
};

struct GfxBackEndPrimitiveData {
	int hasSunDirChanged;
};


struct GfxBackEndData {
	unsigned __int8 sceneLightTechType[13][256];
	unsigned int sparkSurfs[64];
	GfxViewParms viewParms[4];
	GfxMeshData mesh[5];
	int localClientNum;
	GfxBackEndPrimitiveData prim;
	volatile int bspSurfDataUsed;
	volatile int smodelSurfDataUsed;
	volatile int smodelSurfVisDataUsed;
	unsigned int sceneLightHasShadowMap[8];
	int drawSurfCount;
	volatile int surfPos;
	volatile int gfxEntCount;
	unsigned int codeSurfCount[2];
	unsigned int codeSurfArgsCount[2];
	volatile int cloudDataCount;
	unsigned int glassSurfCount;
	unsigned int markSurfCount;
	volatile int sparkSurfCount;
	GfxVertexBufferState *skinnedCacheVb;
	unsigned int endFence;
	unsigned int endFrameFence;
	int viewParmCount;
	GfxFog fogSettings;
};

struct GfxCmdBufInput : class_helper {

};

