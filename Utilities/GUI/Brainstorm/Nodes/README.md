
```
                  |--------------- Serialized ---------------|
                  
NodeControl ----- NodeControlDataSceneData ------> NodeContent ------- NodeContentControl
```


- `NodeControl` - generic class that understands user selecting, resizing, etc via the `SceneRenderer`.
- `NodeControlData` - the x,y,w,h details of the node that need to be serialized out to disk, and contains the `NodeContent`.
- `NodeContent` - the data inside the node (e.g. in the `FileSystemItem` node, the file path, comment, etc).
- `NodeContentControl` - the control that knows how to render a `NodeContent`.  Note that this must have a public constructor that can accept the correct `NodeContent`.

