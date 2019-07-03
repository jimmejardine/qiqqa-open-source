#ifndef SPDF_H
#define SPDF_H

#if defined(SPDF_EXPORTS)
#define SPDF_API __declspec(dllexport) _stdcall
#elif defined(SPDF_IMPORTS)
#define SPDF_API __declspec(dllimport) _stdcall
#else
#define SPDF_API
#endif

#define SPDINFO_STR_SIZE				0x0400

/*
	Types
*/
typedef HANDLE	HSPDFDOC;

struct tagSPDINFO
{
	char szTitle[SPDINFO_STR_SIZE];
	char szSubject[SPDINFO_STR_SIZE];
	char szKeywords[SPDINFO_STR_SIZE];
	char szProducer[SPDINFO_STR_SIZE];
	char szAuthor[SPDINFO_STR_SIZE];
	char szCreationDate[SPDINFO_STR_SIZE];
	char szCreator[SPDINFO_STR_SIZE];	
	char szModDate[SPDINFO_STR_SIZE];
};

typedef struct tagSPDINFO	SPDINFO;
typedef struct tagSPDINFO *PSPDINFO;

struct tagNMSPVACTION
{
    NMHDR hdr;
	LPARAM lParam; 
};

typedef struct tagNMSPVACTION	NMSPVACTION;
typedef struct tagNMSPVACTION *PNMSPVACTION;

typedef BOOL (CALLBACK *SPDOUTLINEPROC)(wchar_t* /* lpszTitle */,
										UINT /* nLevel */,
										LPVOID  /* lpvID */,
										BOOL /* bExpanded */,
										LPVOID /* lpvParam */);

typedef BOOL (CALLBACK *SPDEXPORTPROC)(int /* nPage */,
							 LPVOID /* lpvParam */);

typedef BOOL (CALLBACK *SPDFONTINFOPROC)(LPSTR /* lpszFontName */,
								DWORD /* dwFontFlags */,								 
								LPSTR /* lpszActFontName */,
								DWORD /* dwActFontFlags */,
								int /* nCurPage */,
								LPVOID /* lpvParam */);
/*
	Export types
*/
#define SPD_EXPORT_TYPE_TXT				0x01
#define SPD_EXPORT_TYPE_BMP				0x02
#define SPD_EXPORT_TYPE_XML				0x03

/*
	Encryption Flags
*/
#define SPD_ENF_OPEN_PWD				0x0001
#define SPD_ENF_PERM_PWD				0x0002
#define SPD_ENF_ALGORITHM_RC4			0x0010
#define SPD_ENF_ALGORITHM_AES			0x0020
#define SPD_ENF_KEYLENGTH_40			0x0100
#define SPD_ENF_KEYLENGTH_128			0x0200

/*
	Permission Flags
*/
#define SPD_PEF_PRINT					0x0004
#define SPD_PEF_MODIFY					0x0008			
#define SPD_PEF_COPY					0x0010
#define SPD_PEF_COMMENT					0x0020
#define SPD_PEF_FILL					0x0100
#define SPD_PEF_EXTRACT					0x0200
#define SPD_PEF_ASSEMBLY				0x0400
#define SPD_PEF_PRINTLOW				0x0800

/*
	Font info flags
*/
#define SPD_FIF_TYPE_TYPE1				0x0001
#define SPD_FIF_TYPE_TRUETYPE			0x0002
#define SPD_FIF_TYPE_EMBEDDED			0x0004
#define SPD_FIF_TYPE_SUBSET				0x0008
#define SPD_FIF_TYPE_CID				0x0010
#define SPD_FIF_ENC_ANSI				0x0100
#define SPD_FIF_ENC_IDENTITYH			0x0200
#define SPD_FIF_ENC_IDENTITYV			0x0400
#define SPD_FIF_ENC_BUILTIN				0x0800
#define SPD_FIF_ENC_CUSTOM				0x1000

/*
	Error codes
*/
#define	ERROR_INVALID_FILE				7001
#define	ERROR_INVALID_USER_PASSWORD		7002
#define	ERROR_INVALID_DOCUMENT_HANDLE	7003
#define	ERROR_INVALID_VIEW_HANDLE		7004
#define	ERROR_PAGE_NOT_EXITS			7005
#define	ERROR_PERM_COPY					7006
#define	ERROR_PERM_PRINT				7007
#define ERROR_PRINTER_INFO				7008

#define ERROR_EXPORT_CCO				7021
#define ERROR_EXPORT_CIO				7022

#define ERROR_EXP_BMP_NEM				7031
#define ERROR_EXP_BMP_FILECREATE		7032

/*
	Viewer messages
*/
#define WM_PD_ATTACH					WM_USER + 0x01
#define WM_PD_DETACH					WM_USER + 0x02

#define WM_PV_SETCOLOR					WM_USER + 0x03
#define WM_PV_SETBORDER					WM_USER + 0x04

#define WM_PV_SHOWPAGE					WM_USER + 0x05
#define WM_PV_GETNEXTPAGE				WM_USER + 0x06
#define WM_PV_GETPREVPAGE				WM_USER + 0x07
#define WM_PV_GETSCALE					WM_USER + 0x08
#define WM_PV_HASSELECTION				WM_USER + 0x09
#define WM_PV_SELECTALL					WM_USER + 0x0A
#define WM_PV_COPY						WM_USER	+ 0x0B

#define WM_PV_PRINT						WM_USER	+ 0x0C
#define WM_PV_FIND						WM_USER	+ 0x0D

#define WM_PV_DOC2WND					WM_USER	+ 0x0E
#define WM_PV_WND2DOC					WM_USER	+ 0x0F

#define WM_PV_ROTATEVIEW				WM_USER	+ 0x10
#define WM_PV_ROTATEPAGE				WM_USER	+ 0x11

#define WM_PV_DOOUTLINEACTION			WM_USER + 0x12
#define WM_PV_DOOPENACTION				WM_USER + 0x13

#define WM_PV_ACTIVATETOOL				WM_USER + 0x14
#define WM_PV_SETVIEWLAYOUT				WM_USER + 0x15
#define WM_PV_SETVIEWMODE				WM_USER + 0x16

#define WM_PV_BMIDTOPAGE				WM_USER + 0x17
#define WM_PV_GETCURPAGE				WM_USER + 0x18
#define WM_PV_GETDISPRECT				WM_USER + 0x19

#define WM_PV_SHOWDISPRECT				WM_USER + 0x1A
#define WM_PV_AUTOSCROLL				WM_USER + 0x1B

#define WM_PV_GETSELRECT				WM_USER + 0x1C
#define WM_PV_SETSELRECT				WM_USER + 0x1D
#define WM_PV_SHOWSELECTION				WM_USER + 0x1E
#define WM_PV_GETSELTEXT				WM_USER + 0x1F
#define WM_PV_INFSELRECT				WM_USER + 0x20

#define WM_PV_ALLOWANNOTACTION			WM_USER + 0x21

/*
	Selection messages (for WM_PV_INFSELRECT)
*/
#define PVIS_LEFT						0x01
#define PVIS_RIGHT						0x02

/*	
	Notifycation messages
*/
#define PVN_OPEN_FILE					0x01
#define PVN_PAGE_CHANGED				0x02
#define PVN_SCROLLPOS_CHANGED			0x03
#define PVN_RUN_APP						0x04
#define PVN_GO_BACK						0x05
#define PVN_GO_FORWARD					0x06
#define PVN_QUIT						0x07

/*
	Color settings
*/
#define PVC_BKGND						0x01
#define PVC_PAPER						0x02
#define PVC_PAPER_FRAME					0x03
#define PVC_BORDER						0x04
#define PVC_FOCUS						0x05
#define PVC_HOVER						0x06

/*
	Window border
*/

#define PVB_NONE						0x00
#define PVB_LEFT						0x01
#define PVB_TOP							0x02
#define PVB_RIGHT						0x04
#define PVB_BOTTOM						0x08
#define PVB_ALL		\
	(PVB_LEFT | PVB_TOP | PVB_RIGHT | PVB_BOTTOM)

/*
	View layout
*/
#define PVL_GETVIEWLAYOUT				0x00
#define PVL_SINGLEPAGE					0x01
#define PVL_TWOUP						0x02
#define PVL_CONTINUOUS					0x04
#define PVL_SHOWHGAP					0x10
#define PVL_SHOWVGAP					0x20
#define PVL_SHOWCOVER					0x40

/*
	Find parameters
*/
#define PVFP_OINIT						1
#define PVFP_GONEXT						-1
#define PVFP_GETTEXT					-2
#define PVFP_SETTEXT					-3
#define PVFP_ABORT						-4
#define PVFP_ALL						-5

/*
	View mode
*/
#define PVM_GETVIEWMODE					0x00
#define PVM_NORMAL						0x01
#define PVM_FULLSCREEN					0x02

/*	
	View scale
*/
#define PVS_ZOOM						0
#define PVS_ACTUALSIZE					-1
#define PVS_FITWIDTH					-2
#define PVS_FITPAGE						-3
#define PVS_FITHEIGHT					-4

/*
	Rotate view/page
*/
#define PVR_GETCURRENT					-1

/* 
	Auto Scroll Flags
*/
#define PVASF_GETSTATE					0x00
#define PVASF_SCROLL					0x01
#define PVASF_DIR_UP					0x02
#define PVASF_DIR_DOWN					0x04

/* 
	Printing contstans
*/
#define PVPF_MODE_BMP					0x0001
#define PVPF_MODE_PS					0x0002

#define PVPF_PSF_LEVEL1					0x0010
#define PVPF_PSF_LEVEL2					0x0020
#define PVPF_PSF_LEVEL3					0x0040
#define PVPF_PSF_SEPARATION				0x0080

#define PVPF_EMB_PSFONTS				0x0F00
#define PVPF_DUPLEX						0x4000

#define PVPF_QUICKPRINT					0x8000

#define PVPF_DP_LANDSCAPE				0x8000

/*
	Cursor tools
*/
#define PVCT_GETACTIVETOOL				0x00
#define PVCT_HAND						0x01
#define PVCT_SELECT						0x02
#define PVCT_ZOOM_MARQUEE				0x03
#define PVCT_ZOOM_DYNAMIC				0x04
#define PVCT_SNAPSHOT					0x05

/*
	Allow Annot actions
*/
#define PVAA_GETPERMFLAGS				-1
#define PVAA_ALLOW_INTERNALLINK			0x01
#define PVAA_ALLOW_EXTERNALLINK			0x02
#define PVAA_ALLOW_WIDGET				0x04
#define PVAA_ALLOW_ALL	\
	(PVAA_ALLOW_INTERNALLINK | PVAA_ALLOW_EXTERNALLINK | \
	PVAA_ALLOW_WIDGET)

#ifdef __cplusplus
extern "C"
{
#endif

BOOL SPDF_API SPD_ResetConfig(LPSTR lpszFileName);

HSPDFDOC SPDF_API SPD_Open(LPSTR lpszFileName,
								 LPSTR lpszUserPwd,
								 LPSTR lpszOwnerPwd);
BOOL SPDF_API SPD_Close(HSPDFDOC hDoc);

BOOL SPDF_API SPD_SaveAs(HSPDFDOC hDoc, LPSTR lpszFileName);

int SPDF_API SPD_GetPageCount(HSPDFDOC hDoc);
int SPDF_API SPD_GetPageText(HSPDFDOC hDoc, int nPage,
							 LPSTR lpszBuf, size_t nSize);
BOOL SPDF_API SPD_GetPageSize(HSPDFDOC hDoc, int nPage,
							 LPSIZE lpSize);
int SPDF_API SPD_GetPageLabel(HSPDFDOC hDoc, int nPage,
							 LPSTR lpszBuf, size_t nSize);

BOOL SPDF_API SPD_GetInfo(HSPDFDOC hDoc, PSPDINFO pInfo);
float SPDF_API SPD_GetVersion(HSPDFDOC hDoc);
DWORD SPDF_API SPD_GetEncFlags(HSPDFDOC hDoc);
DWORD SPDF_API SPD_GetPermFlags(HSPDFDOC hDoc);
BOOL SPDF_API SPD_GetFonts(HSPDFDOC hDoc, int nFromPage, int nToPage, 
						   SPDFONTINFOPROC pCb, LPVOID pvParam);
int SPDF_API SPD_GetMetaData(HSPDFDOC hDoc, LPTSTR lpszBuf,
							 size_t nSize);
int SPDF_API SPD_GetOutline(HSPDFDOC hDoc, SPDOUTLINEPROC pCb,
							LPVOID pvParam);
BOOL SPDF_API SPD_Export(HSPDFDOC hDoc, LPTSTR lpszFileName,
						 int nFromPage, int nToPage, int nExpType,
						 SPDEXPORTPROC pCb, LPVOID pvParam);
BOOL SPDF_API SPD_PrintDirect(HSPDFDOC hDoc, LPSTR lpszPrinterName,
							  int nFromPage, int nToPage, WORD wFlags);

HWND SPDF_API SPV_Create(HWND hParentWnd, LPCRECT lpRect, UINT uID);

#ifdef __cplusplus
}
#endif

#endif
