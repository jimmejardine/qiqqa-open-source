# Qiqqa Legacy Formats library

This library stores the source code to read (and possibly write) legacy Qiqqa file formats, e.g. protobuf and objectserializer based binary file formats for the Qiqqa application configuration, etc.

This keeps those concerns (old code, structures which cannot be updated without breaking the serialization on disk, etc.) separate from the actual Qiqqa code.

