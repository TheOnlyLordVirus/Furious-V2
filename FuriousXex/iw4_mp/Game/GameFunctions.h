#pragma once
#include <xtl.h>

typedef unsigned int uint32_t;
typedef unsigned int Font_s;
typedef unsigned int Material_Test;
typedef unsigned int DObj;
typedef unsigned int cpose_t;
typedef unsigned int playerState_s;

#define FONT_NORMAL "fonts/normalFont"
#define FONT_BIGDEV "fonts/bigDevFont"
#define FONT_SMALLDEV "fonts/smallDevFont"
#define FONT_CONSOLE "fonts/consoleFont"
#define FONT_OBJECTIVE "fonts/objectiveFont"
#define FONT_EXTRABIG "fonts/extraBigFont"
#define FONT_BOLD "fonts/boldFont"
#define FONT_SMALL "fonts/smallFont"
#define FONT_BIG "fonts/bigFont"

#define MATERIAL_WHITE "white"

extern int *screen_res;

enum eAlignH {
	ALIGN_LEFT = (10 << 0),
	ALIGN_CENTER = (10 << 1),
	ALIGN_RIGHT = (10 << 2)
};

enum eAlignV {
	ALIGN_TOP = (10 << 0),
	ALIGN_MIDDLE = (10 << 1),
	ALIGN_BOTTOM = (10 << 2)
};


int Sys_Milliseconds();
Font_s* CL_RegisterFont(const char *fontName, int imageTrack);
Material_Test* Material_RegisterHandle(const char *name, int imageTrack);
int R_TextWidth(const char *text, int maxChars, Font_s *font);
int R_TextHeight(Font_s* font);

void R_AddCmdDrawText(const char *text, int maxChars, Font_s *font, float x, float y, float xScale, float yScale, float rotation, const float *color, int style);
void R_AddCmdDrawStretchPic(float x, float y, float w, float h, float s0, float t0, float s1, float t1, const float *color, Material_Test *material);

void DrawText(const char * text, float x, float y, float scale, const char * fontName, const float * color, eAlignH alignx, eAlignV aligny);
void DrawShader(float x, float y, float w, float h, const float * color, const char * imageName, eAlignH alignx, eAlignV aligny);

const char * SEH_StringEd_GetString(const char *pszReference);
void Cmd_ExecuteSingleCommand(int localClientNum, int controllerIndex, const char *text);
void DObjSetThermalRender(DObj *obj, bool enabled);

