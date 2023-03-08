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
- [X] Sub Canvas
>关于Image的合批，需要就SpriteAtlas做几点说明<br>
> 1）Unity仅在在Play模式下为SpriteAtlas生成对应的Texture，所以本工具在Editor模式下以SpriteAtlas替代运行时（真机或Play模式）动态生成的对应的Texture。可能导致Batch的前后顺序有差异，但不影响合批的本质<br>
> 2）如果SpriteAtlas的尺寸设置的太小，会在运行时生成多个Texture，导致即使多个Image引用同一个SpriteAtlas中的不同Sprites也无法合批。目前无法知晓某个Sprite具体在哪一个Texture中，所以在Editor模式下得到的合批结果可能和运行时不一致<br>
> 3）在Play模式下，是否会为SpriteAtlas生成Texture是需要在 **Project Settings->Editor->Sprite Packer** 中设置的（默认是Disable）
## 使用指南
1. 菜单 SimpleX -> UIBatch Profiler，打开UGUI Batch窗口<br>![ugui_batch_03.png](https://github.com/simplex86/UIBatchAnalyzer/blob/main/_Doc/Images/ugui_batch_03.png)
2. 点击“Enable”按钮，启动Batch分析功能<br>![ugui_batch_04.png](https://github.com/simplex86/UIBatchAnalyzer/blob/main/_Doc/Images/ugui_batch_04.png)
3. 左边列出每个Canvas下的Batch及其UI控件的组成。选择各个节点后，可以在右边查看详细信息<br>![ugui_batch_05.png](https://github.com/simplex86/UIBatchAnalyzer/blob/main/_Doc/Images/ugui_batch_05.png)
4. 点击“Disable”按钮，关闭Batch分析功能
>在Inspector中修改某些属性时，不会自动刷新Batch数据，请重新Enable后查看
## 测试环境
#### Windows
- [X] Unity 2020.3.44f1
- [X] Unity 2021.3.18f1
- [X] Unity 2022.2.8f1
#### Mac
- [ ] Unity 2020
- [ ] Unity 2021
- [ ] Unity 2022
## 参考文献
1. [UGUI drawcall合并](https://blog.csdn.net/akak2010110/article/details/80953370)
2. [UGUI 源码](https://github.com/Unity-Technologies/uGUI)
