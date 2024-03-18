# libglob - does not fare well when traversing non-Unicode-legal filenames

When that happens, it's raining these in MSVC debugger:

```
Exception thrown at 0x00007FF9A0CECF19 in glob.exe: Microsoft C++ exception: std::system_error at memory location 0x000000D7FEF8C4D0.
Unhandled exception at 0x00007FF9A0CECF19 in glob.exe: Microsoft C++ exception: std::system_error at memory location 0x000000D7FEF8C4D0.
```

and you can get lockups inside the MSVC provided std::filesystem implementation.

Example directory name that breaks the bank:

```
Abusing Vector Search for Texts, Maps, and Chess ♟️ _ Ash's Blog_files
```


**Ergo**: while I really like `std::filesystem`, for *some* purposes we SHOULD NOT depend on it and ride the OS native APIs instead, at least on MS Windows.