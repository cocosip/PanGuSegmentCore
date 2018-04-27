using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LucenePanGuDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("盘古分词初始化");
            PanGu.Segment.Init();
            Console.WriteLine("清理索引...");
            ArticleIndexHelper.ClearIndex();
            Console.WriteLine("开始创建索引");

            var article1 = new Article()
            {
                Id = 8600935,
                Title = "浅谈Log4net在项目中如何记录日志",
                Content = "在软件开发周期中，无论是开发中，或是测试中，或是上线后，选择合适的工具监控程序的运行状态至关重要，只有如此，才能更好地排查程序问题和检测程序性能问题等。本篇文章主要与大家分享，如何利用Log4net在项目中记录日志文件。在应用程序出现问题时，启用日志记录有助于解决问题。使用log4net，可以在运行时启用日志记录，而无需修改应用程序二进制文件。log4net是帮助程序员输出日志语句到各种输出目标的工具。",
                Author = "Alan_beijing"
            };
            var article2 = new Article()
            {
                Id = 8962205,
                Title = "Jmeter_ForEach控制器实现网页爬虫",
                Content = "一直以来，爬虫似乎都是写代码去实现的，今天像大家介绍一下Jmeter如何实现一个网页爬虫！Jmeter的爬虫原理其实很简单，就是对网页提交一个请求，然后把返回的所有href提取出来，利用ForEach控制器去实现url遍历。这样解释是不是很清晰？下面就来简单介绍一下如何操作。首先我们需要对网页提交一个请求，就拿腾讯新闻网举例子吧！我们像腾讯新闻网发起一个请求，观察一下返回值可以发现中间有很多href标签 + 文字标题的url",
                Author = "飞天小子"
            };
            var article3 = new Article()
            {
                Id = 8949393,
                Title = "手把手教你写网络爬虫（7）：URL去重",
                Content = "本期我们来聊聊URL去重那些事儿。以前我们曾使用Python的字典来保存抓取过的URL，目的是将重复抓取的URL去除，避免多次抓取同一网页。爬虫会将待抓取的URL放在todo队列中，从抓取到的网页中提取到新的URL，在它们被放入队列之前，首先要确定这些新的URL是否被抓取过，如果之前已经抓取过了，就不再放入队列。有别于单机系统，在分布式系统中，这些URL应该存放在公共缓存中，才能让多个爬虫实例共享，我们继续使用Redis缓存这些数据。URL既可以存储在Redis的Set数据结构中，也可以将URL作为Key存储为Redis的String类型。至于这两种方案各有什么优缺点，就留给读者自己去思考了。将URL以字符串的形式直接存储到内存中。保守估计一下URL的平均长度是100字节，那么1亿个URL所占的内存是: 100000000 * 0.0001MB = 10000MB，约等于10G。这也不是不能用，占用的空间再大都能通过扩容来解决。问题是，如果一个服务器存不下这么多URL该怎么办呢？其实也简单，明确每台服务器的分工，也就是说得到一个URL就知道要交给哪台服务器存储，每台服务器只存储一类URL，比较简单的实现方式就是对URL先哈希再取模。虽然能用，但还是有很大优化空间的。",
                Author = "拓海"
            };
            var article4 = new Article()
            {
                Id = 6936136,
                Title = "浅谈微服务的来龙去脉",
                Content = "最近一段时间公共业务平台在进行大面积的重构，对原来的技术栈进行迁移，逐渐往java、go、node.js等开源、自由为主的技术体系中过度。虽然这主要是替换技术框架，但也是我们应用系统进行重新设计、业务流程重新梳理的一个好机会，我们将利用这次机会来重构之前发现的一些问题。Martin Fowler大师《重构》一书中有说过一句话，大概意思就是，“每次对原有系统进行修改调整的时候是一个非常好的重构契机。”对一个正在运行良好的系统进行重构机会不多，只有这种需要对系统进行适当的修改或者调整的时候，刚好可以借机重构。因为你如果总是不抓住这样难得的重构机会久而久之，就意味着你之前积累的技术债务永远偿还不了，最后就是频繁的破窗效应、墨菲定律。我们正视前行的道路上欠下的技术债务，把握恰当的时机及时偿还，这样才有可能不会因为技术包袱影响系统建设，影响业务发展。这些在George Fairbanks 大师的《恰如其分的软件架构》一书中讲解的很深刻，最重要的一条就是“风险驱动”的架构设计原则，一定要先识别出风险，针对风险排好优先级及时重构解决。我们还面临着一个最大的问题就是，需要一边支持业务高速发展一边进行系统重构，更要命的是还需要支持各业务线进行各种大促活动，配合进行核心交易系统压测等等，可想而知这种压力还是不小。而且我们是沪江集团的公共业务平台，我们是服务于沪江集团所有业务线，包括B2C业务（沪江网校）、直播业务（CCtalk）、外部订单业务（开放平台）、UGC（沪江问答）、批量导入订单（TPS峰值较高）等核心业务。大家可能对传统电商的业务比较了解，但是对教育行业的类电商业务可能不是很了解，两者之间最大的一个区别就是在商品的标准化上。教育是以服务为主，不是一个简单的买卖商品的过程，而是一个智能推荐的过程，所有的商品都是需要基于用户之前的学习数据来动态生成，也就是说，公共业务平台的商品系统中的商品是动态变化的，没有固定属性的商品，商品的属性一直随着数据积累在进化。这就带来一个巨大的挑战，因为一旦不是基于一个固定标准的商品来进行交易系统、商品系统、营销系统、商户清结算系统等核心系统设计时会面临着一个极大的抽象难度，必须建立起一套庞大且可以落地的抽象模型出来，足以容纳那些变化万千的业务模式。这方面的系统建设还没有太多成熟的模式可以参考，这是一个考验你综合技术能力的时候，光有一个“具体”的技术是解决不了这种问题域的。这个背景介绍主要是为了能够与读者达成一个基本技术讨论的上下文环境，目的是为了在讲解本文主题的时候大家在一个频道上，这样才不会浪费大家宝贵的时间。通过背景介绍，至少我们达成以下共识，我们正在做系统重构，业务主要是提供服务型商品，背后需要很多智能化的支持，同时我们是平台型的系统，所以我们会陆续面临平台型系统所碰到的所有问题。本文仅仅代表个人对应用架构、软件工程的一些独立思考的总结",
                Author = "王清培"
            };
            ArticleIndexHelper.CreateIndex(article1);
            Console.WriteLine("创建文章1索引");
            ArticleIndexHelper.CreateIndex(article2);
            Console.WriteLine("创建文章2索引");
            ArticleIndexHelper.CreateIndex(article3);
            Console.WriteLine("创建文章3索引");
            ArticleIndexHelper.CreateIndex(article4);
            Console.WriteLine("创建文章4索引");
            Console.WriteLine("创建索引完成......");


            while (true)
            {
                Console.WriteLine($"请输入搜索的内容:");
                var q = Console.ReadLine();
                Console.WriteLine($"您输入的为:{q},开始进行检索......");
                Console.WriteLine("检索结果:");
                var productIndexs = ArticleIndexHelper.Search(q);
                productIndexs.ForEach(x =>
                {
                    Console.WriteLine($"文章标题:{x.Title},作者:{x.Author},文章Id:{x.Id}");
                });
            }
        }
    }
}
