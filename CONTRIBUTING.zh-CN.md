# 贡献给 PDF 补丁丁

欢迎使用 PDF 补丁丁! 本文档是有关如何为 PDF 补丁丁 做出贡献的指南。如果发现不正确或缺失的内容，请留下评论/建议。

## 开始之前

### 设置您的开发环境

请参阅 [README](https://github.com/wmjordan/PDFPatcher#%E8%BF%90%E8%A1%8C%E7%8E%AF%E5%A2%83)。

## 贡献

无论是对于拼写错误，Bug 修复还是重要的新功能，我们总是很乐意接受您的贡献。请不要犹豫，在 [Github Issue](https://github.com/wmjordan/PDFPatcher/issues) 上进行讨论。

我们非常重视文档，我们很高兴接受这些方面的改进。

### GitHub 工作流程

我们将 `master` 分支用作开发分支。

这是贡献者的工作流程 :

复刻 (Fork) 到您的个人仓库。

克隆到本地。

```git
git clone git@github.com:yourgithub/PDFPatcher.git
```

创建一个新分支并对其进行处理。

```git
git checkout -b your_branch
```

(不建议将 `master` 作为 `your_branch`。)

保持分支与上游仓库同步：

```git
git remote add upstream git@github.com:wmjordan/PDFPatcher.git
git remote update
git rebase upstream/master
```

提交您的更改 (确保您的提交说明清晰完整)。

```git
git commit -a
```

整理提交，然后将您的提交推送到复刻的存储库。

```git
git push origin your_branch
```

创建 Pull request 合并请求。

后续在修改更改后，一般应强制推送到复刻的存储库：

```git
git push origin your_branch -f
```

请确保 PR 对应有相应的 Issue。请参阅 [将拉取请求链接到议题 - GitHub Docs](https://docs.github.com/cn/issues/tracking-your-work-with-issues/linking-a-pull-request-to-an-issue) 。

创建 PR 后，社区会有成员帮助 Review，Review 通过之后，PR 将会合并到主仓库，相应的 Issue 会被关闭。

### 打开 Issue/PR

我们使用 Issue 和 Pull Requests 作为跟踪器：

- [GitHub Issues](https://github.com/wmjordan/PDFPatcher/issues)
- [Pull Requests](https://github.com/wmjordan/PDFPatcher/pulls)

如果您发现新的 Bug，想要新功能或提出新当建议，您可以在 GitHub 上[创建 Issue](https://github.com/wmjordan/PDFPatcher/issues/new/choose) ，请按照 Issue 模板中的准则进行操作。

如果您在文档中发现拼写错误，或者发现代码中存在可以进行微小的优化的地方，您可以无需创建 Issue， 直接提交一个 PR。

如果您想贡献，请先创建一个新的 PR。 如果您的 PR 包含较大的更改，请写详细描述有关其设计和使用的信息。

> **注意**
>
> 单个 PR 不应太大。如果需要进行重大更改，最好将更改分开到一些 PR。

### PR 审查

所有 PR 应进行良好的审查。一些原则:

- 易用性。更改不应对软件的易用性产生负面作用。

- 第三方代码尽量保持原状。PDFPatcher 命名空间外的代码为第三方代码，如非必要尽量不修改。

### PR 合并

PR 经过 Approve 之后会由 Committer 负责合并，在合并的时候，Committer 可以对提交说明进行修改。
在合并时一般使用 Rebase and merge。对于大型多人协助的 PR，使用 Merge 进行合并，在合并之前通过 Rebase 修正提交。
