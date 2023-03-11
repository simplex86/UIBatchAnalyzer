# UGUI合批分析工具
优化UGUI的Drawcall时，一般使用Unity提供的FrameDebugger来查看合批情况。但就我个人使用来看，用FrameDebugger优化UI有以下几个不足
>1. 不够直观，不能快速定位构成Drawcall的控件
>2. 在编辑模式下，对SpriteAtlas的支持并不好

所以开发了这个分析UGUI合批情况的工具，可以作为FrameDebugger的辅助和补充

![ugui_batch_02.png](https://github.com/simplex86/UIBatchAnalyzer/blob/main/_Doc/Images/ugui_batch_02.png)

## Feature
- [X] Text (Legacy)
- [X] TextMeshPro
- [X] Image
- [X] RawImage
- [X] Mask
- [X] RectMask2D
- [X] Nested Canvas
- [ ] 3D UI
> Position的Z轴不为0或Rotation的Y轴不为0时，UGUI会把该节点及其子节点当做**3D UI**，它们不参与2D UI的合批。具体的合批规则还没有查到确信的资料，还在试验总结其规律。<br>所以在使用UGUI拼界面时，确保各个节点Position的Z轴和Rotation的Y轴的值为0，避免增加不必要的Drawcall。

## 使用指南
1. 菜单 SimpleX -> UIBatch Profiler，打开UGUI Batch窗口<br>![ugui_batch_03.png](https://github.com/simplex86/UIBatchAnalyzer/blob/main/_Doc/Images/ugui_batch_03.png)
2. 点击“Enable”按钮，启动Batch分析功能<br>![ugui_batch_04.png](https://github.com/simplex86/UIBatchAnalyzer/blob/main/_Doc/Images/ugui_batch_04.png)
3. 左边列出每个Canvas下的Batch及其UI控件的组成。选择各个节点后，可以在右边查看详细信息<br>![ugui_batch_05.png](https://github.com/simplex86/UIBatchAnalyzer/blob/main/_Doc/Images/ugui_batch_05.png)
4. 点击“Disable”按钮，关闭Batch分析功能
>如果在Inspector中修改某些属性后没有自动刷新Batch数据，请手动刷新（先Disable，再Enable）

## 重要说明
通常使用Image的初衷是利用SpriteAtlas来降低Drawcall，但SpriteAtlas存在几个特性
> 1. 编辑器在 **非Play时** 不会为SpriteAtlas实时生成Texture，此时会直接引用Sprite对应的Texture （当 **Project Settings->Editor->Sprite Packer** 设置为 **Always Enabled** 才会在 **Play时** 生成Texture）
> 2. 如果SpriteAtlas的尺寸设置得太小，可能会生成多张Texture
> 3. 如果某个Sprite同时Pack到多个SpriteAtlas中，无法判断在运行时会使用哪个SpriteAtlas生成的Texture

由此，在 **非Play时** UI合批出现以下几个事实
> 1. 在FrameDebugger中，引用不同Sprite的Image无法合批
> 2. 本工具尝试用SpriteAtlas代替Texture作为能否合批的依据，但这**并不完全准确**

因此建议修改UI优化Drawcall后，Play起来确定最终的结果

## 测试环境
#### Windows
- [X] Unity 2020.3.44f1
- [X] Unity 2021.3.6f1
- [X] Unity 2021.3.18f1
- [X] Unity 2022.2.8f1
#### Mac
- [ ] Unity 2020
- [ ] Unity 2021
- [ ] Unity 2022

## 参考文献
1. [UGUI drawcall合并](https://blog.csdn.net/akak2010110/article/details/80953370)
2. [UGUI 源码](https://github.com/Unity-Technologies/uGUI)
