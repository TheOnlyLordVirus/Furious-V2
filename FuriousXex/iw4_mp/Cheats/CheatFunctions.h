#pragma once

#include "Game/GameFunctions.h"
#include "Game/HudElems.h"
#include "Game/GameStructs.h"
#include "Math/CheatMath.h"
#include "Addresses.h"

typedef struct
{
	int button_a;//0x0 | 0x825DCAA4
	char padding0[0x8]; //0x4
	int button_b;//0xC | 0x825DCAB0
	char padding1[0x8]; //0x10
	int button_x;//0x18 | 0x825DCABC
	char padding2[0x8]; //0x1C
	int button_y;//0x24 | 0x825DCAC8
	char padding3[0x8]; //0x28
	int button_ltrig;//0x30 | 0x825DCAD4
	char padding4[0x8]; //0x34
	int button_rtrig;//0x3C | 0x825DCAE0
	char padding5[0x5C]; //0x40
	int button_start;//0x9C | 0x825DCB40
	char padding6[0x8]; //0xA0
	int button_back;//0xA8 | 0x825DCB4C
	char padding7[0x8]; //0xAC
	int button_lstick;//0xB4 | 0x825DCB58
	char padding8[0x8]; //0xB8
	int button_rstick;//0xC0 | 0x825DCB64
	char padding9[0x8]; //0xC4
	int button_lshldr;//0xCC | 0x825DCB70
	char padding10[0x8]; //0xD0
	int button_rshldr;//0xD8 | 0x825DCB7C
	char padding11[0x8]; //0xDC
	int dpad_up;//0xE4 | 0x825DCB88
	char padding12[0x8]; //0xE8
	int dpad_down;//0xF0 | 0x825DCB94
	char padding13[0x8]; //0xF4
	int dpad_left;//0xFC | 0x825DCBA0
	char padding14[0x8]; //0x100
	int dpad_right;//0x108 | 0x825DCBAC
}ButtonMonitor;

enum ButtonIndex
{
	button_a = 0x0,
	button_b = 0x0C,
	button_x = 0x18,
	button_y = 0x24,
	button_ltrig = 0x30,
	button_rtrig = 0x3C,
	button_start = 0x9C,
	button_back = 0xA8,
	button_lstick = 0xB4,
	button_rstick = 0xC0,
	button_lshldr = 0xCC,
	button_rshldr = 0xD8,
	dpad_up = 0xE4,
	dpad_down = 0xF0,
	dpad_left = 0xFC,
	dpad_right = 0x108,
};


struct RGB {
	float r, g, b;
};

typedef struct RainbowRGB
{
	float rgb[4];
	int rgbI[4];
	bool setRGB;
	bool setRGBi;
	float* run(int changeVal)//values: 1, 3, 5, 15 | 1 = slowest 15 = fastest
	{
		if (!setRGB)
		{
			this->setRGB = true;
			this->rgb[0] = 255;
			this->rgb[1] = 0;
			this->rgb[2] = 0;
			this->rgb[3] = 255;
		}
		if (this->rgb[0] == 255 && this->rgb[1] < 255 && this->rgb[2] == 0)
			this->rgb[1] += changeVal;
		if (this->rgb[0] > 0 && this->rgb[1] == 255 && this->rgb[2] == 0)
			this->rgb[0] -= changeVal;
		if (this->rgb[0] == 0 && this->rgb[1] == 255 && this->rgb[2] < 255)
			this->rgb[2] += changeVal;
		if (this->rgb[0] == 0 && this->rgb[1] > 0 && this->rgb[2] == 255)
			this->rgb[1] -= changeVal;
		if (this->rgb[0] < 255 && this->rgb[1] == 0 && this->rgb[2] == 255)
			this->rgb[0] += changeVal;
		if (this->rgb[0] == 255 && this->rgb[1] == 0 && this->rgb[2] > 0)
			this->rgb[2] -= changeVal;

		return this->rgb;
	}

	int* runI(int changeVal)//values: 1, 3, 5, 15 | 1 = slowest 15 = fastest
	{
		if (!setRGBi)
		{
			this->setRGBi = true;
			this->rgbI[0] = 255;
			this->rgbI[1] = 0;
			this->rgbI[2] = 0;
			this->rgbI[3] = 255;
		}
		if (this->rgbI[0] == 255 && this->rgbI[1] < 255 && this->rgbI[2] == 0)
			this->rgbI[1] += changeVal;
		else if (this->rgbI[0] > 0 && this->rgbI[1] == 255 && this->rgbI[2] == 0)
			this->rgbI[0] -= changeVal;
		else if (this->rgbI[0] == 0 && this->rgbI[1] == 255 && this->rgbI[2] < 255)
			this->rgbI[2] += changeVal;
		else if (this->rgbI[0] == 0 && this->rgbI[1] > 0 && this->rgbI[2] == 255)
			this->rgbI[1] -= changeVal;
		else if (this->rgbI[0] < 255 && this->rgbI[1] == 0 && this->rgbI[2] == 255)
			this->rgbI[0] += changeVal;
		else if (this->rgbI[0] == 255 && this->rgbI[1] == 0 && this->rgbI[2] > 0)
			this->rgbI[2] -= changeVal;

		return this->rgbI;
	}
} RainbowRGB;

typedef struct ColorShade
{
	color Change(color Color, float correctionFactor)
	{
		float red = Color.r;
		float green = Color.g;
		float blue = Color.b;
		correctionFactor *= 3;
		if (correctionFactor < 0)
		{
			correctionFactor = 1 + correctionFactor;
			red *= correctionFactor;
			green *= correctionFactor;
			blue *= correctionFactor;
		}
		else
		{
			red = (255 - red) * correctionFactor + red;
			green = (255 - green) * correctionFactor + green;
			blue = (255 - blue) * correctionFactor + blue;
		}
		color shade;
		shade.r = red;
		shade.g = green;
		shade.b = blue;
		shade.a = Color.a;

		return shade;
	}
}ColorShade;



//void R_AddCmdDrawStretchPic_Detour(float* x, float* w, const float* col, materialx* shader);

bool Dvar_GetBool(const char* dvarName);
uint32_t floatColorToUInt(const float* color);
void SetGodmode(bool value);
void SetInfAmmo(bool value);
void SetAimbot(bool value);
void loopFunc();
bool abStart;
bool inGame;