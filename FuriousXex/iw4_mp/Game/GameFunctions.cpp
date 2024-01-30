#include "GameFunctions.h"
#include "..//EntryPoint.h"
#include "..//Cheats/Addresses.h"
#include "..//Cheats/Calls.h"

int *screen_res;

int Sys_Milliseconds() {
	auto func = reinterpret_cast<int(*)()>(0x822CF470);
	return func();
}

Font_s* CL_RegisterFont(const char *fontName, int imageTrack) {
	auto func = reinterpret_cast<Font_s*(*)(const char*, int)>(0x82146458);
	return func(fontName, imageTrack);
}

Material_Test* Material_RegisterHandle(const char *name, int imageTrack) {
	auto func = reinterpret_cast<Material_Test*(*)(const char*, int)>(0x8234E510);
	return func(name, imageTrack);
}

int R_TextWidth(const char *text, int maxChars, Font_s *font) {
	auto func = reinterpret_cast<int(*)(const char*, int, Font_s*)>(0x8234DD20);
	return func(text, maxChars, font);
}

int R_TextHeight(Font_s* font) {
	auto func = reinterpret_cast<int(*)(Font_s*)>(0x8234DE10);
	return func(font);
}

void R_AddCmdDrawText(const char *text, int maxChars, Font_s *font, float x, float y, float xScale, float yScale, float rotation, const float *color, int style) {
	auto func = reinterpret_cast<void(*)(const char*, int, Font_s*, float, float, float, float, float, const float*, int)>(0x82350278);
	func(text, maxChars, font, x, y, xScale, yScale, rotation, color, style);
}

void R_AddCmdDrawStretchPic(float x, float y, float w, float h, float s0, float t0, float s1, float t1, const float *color, Material_Test *material) {
	auto func = reinterpret_cast<void(*)(float, float, float, float, float, float, float, float, const float*, Material_Test*)>(0x8234F9B8);
	func(x, y, w, h, s0, t0, s1, t1, color, material);
}

void DrawText(const char * text, float x, float y, float scale, const char * fontName, const float * color, eAlignH alignx, eAlignV aligny) {

	int textLen = strlen(text);
	Font_s* font = CL_RegisterFont(fontName, 0);

	float textWidth = R_TextWidth(text, textLen, font) * scale;
	float textHeight = R_TextHeight(font) * scale;

	switch (alignx) {
		case ALIGN_LEFT: x -= 0; break;
		case ALIGN_CENTER: x -= (textWidth / 2); break;
		case ALIGN_RIGHT: x -= textWidth; break;
		default: break;
	}
	switch (aligny) {
		case ALIGN_BOTTOM: y -= textHeight; break;
		case ALIGN_CENTER: y -= (textHeight / 2); break;
		case ALIGN_TOP: y -= 0; break;
	default: break;
	}

	R_AddCmdDrawText(text, textLen + 1, font, x, y, scale, scale, 0, color, 0);
}

void DrawShader(float x, float y, float w, float h, const float * color, const char * imageName, eAlignH alignx, eAlignV aligny) {

	Material_Test* Material_Test = Material_RegisterHandle(imageName, 3);

	switch (alignx) {
		case ALIGN_LEFT: x -= 0; break;
		case ALIGN_CENTER: x -= (w / 2); break;
		case ALIGN_RIGHT: x -= w; break;
		default: break;
	}
	switch (aligny) {
		case ALIGN_TOP: y -= h; break;
		case ALIGN_CENTER: y -= (h / 2); break;
		case ALIGN_BOTTOM: y -= 0; break;
		default: break;
	}

	R_AddCmdDrawStretchPic(x, y, w, h, 0, 0, 1, 1, color, Material_Test);
}



const char * SEH_StringEd_GetString(const char *pszReference) {
	auto func = reinterpret_cast<const char*(*)(const char*)>(0x82261278);
	return func(pszReference);
}

void Cmd_ExecuteSingleCommand(int localClientNum, int controllerIndex, const char *text) {
	auto func = reinterpret_cast<void(*)(int, int, const char*)>(0x82225930);
	func(localClientNum, controllerIndex, text);
}

void DObjSetThermalRender(DObj *obj, bool enabled) {
	auto func = reinterpret_cast<void(*)(DObj*, bool)>(0x822B0210);
	func(obj, enabled);
}

