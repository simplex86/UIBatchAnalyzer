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
### Windows
- [ ] Unity 2020
- [X] Unity 2021.3.18f1
- [ ] Unity 2022
### Mac
- [ ] Unity 2020
- [ ] Unity 2021.3.18f1
- [ ] Unity 2022
## 说明
基于以下几点原因
1. 没有Unity3D渲染层面的源码
2. 网上资料零散、不全面且无法保证正确性（尤其是Mask相关的资料几乎没有完整且正确的）

只能通过构建Test Case，再由Frame Debugger和UGUI源码，推测UGUI渲染过程。目前这个工具的正确性还不能100%保证，现阶段**建议将它作为Frame Debugger的辅助工具**。
## 参考文献
1. [UGUI drawcall合并](https://blog.csdn.net/akak2010110/article/details/80953370)
2. [UGUI 源码](https://github.com/Unity-Technologies/uGUI)
