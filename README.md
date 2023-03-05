# UGUI合批分析工具
优化UGUI的Drawcall时，一般使用Unity提供的FrameDebugger来查看合批情况。但就我个人使用来看，FrameDebugger优化UI有以下几个不足
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
- [ ] Sub Canvas
>注1：暂不支持多材质UI<br>
>注2：暂不支持Editor模式下的SpriteAtlas<br>
## 使用指南
1. 菜单 SimpleX -> UIBatch Profiler，打开UGUI Batch窗口<br>![ugui_batch_03.png](https://github.com/simplex86/UIBatchAnalyzer/blob/main/_Doc/Images/ugui_batch_03.png)
2. 点击“Sample”按钮<br>![ugui_batch_04.png](https://github.com/simplex86/UIBatchAnalyzer/blob/main/_Doc/Images/ugui_batch_04.png)
3. 左边列出每个Canvas下的所有Batch以及每个Batch的组成。选择各个节点后，可以在右边查看详细信息<br>![ugui_batch_05.png](https://github.com/simplex86/UIBatchAnalyzer/blob/main/_Doc/Images/ugui_batch_05.png)
>注：需要手动点击“Sample”按钮刷新
## 测试环境
#### Windows
- [X] Unity 2020.3.44f1
- [X] Unity 2021.3.18f1
- [ ] Unity 2022
#### Mac
- [ ] Unity 2020
- [ ] Unity 2021
- [ ] Unity 2022
## 参考文献
1. [UGUI drawcall合并](https://blog.csdn.net/akak2010110/article/details/80953370)
2. [UGUI 源码](https://github.com/Unity-Technologies/uGUI)
