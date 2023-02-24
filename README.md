# UGUI合批分析工具
分析UGUI合批情况，比较直观的看到UGUI的每个Batch具体是哪些控件组成，可以作为Frame Debugger的补充

![ugui_batch_01.png](https://github.com/simplex86/UIBatchAnalyzer/blob/main/_Doc/Images/ugui_batch_01.png)
## Feature
- [X] Text
- [X] Image
- [X] RawImage
- [X] Mask
- [ ] RectMask2D
- [ ] Sub Canvas
## 测试环境
Windows + Unity 2021.3.18f1
## 说明
因为没有Unity3D渲染层面的源码，通过网上零零散散、不全面且无法保证正确性的文章（尤其是Mask部分，几乎没有完善且正确的资料），阅读UGUI C#层的源码，同时使用Frame Debugger反复试验，推测和总结UGUI的渲染过程后写出的这个工具。目前还不能保证100%的正确性，正在逐步增加Test Case来查漏补缺。所以，现阶段**建议将它作为Frame Debugger的辅助工具**。
## 参考文献
1. [UGUI drawcall合并](https://blog.csdn.net/akak2010110/article/details/80953370)
2. [UGUI 源码](https://github.com/Unity-Technologies/uGUI)
