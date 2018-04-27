using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using PanGu;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LucenePanGuDemo
{
    /// <summary>文章索引帮助
    /// </summary>
    public class ArticleIndexHelper
    {
        private static LuceneVersion Lucene_Version = LuceneVersion.LUCENE_48;
        private static Lucene.Net.Store.Directory directory;
        //索引存放目录
        private static string DirectoryPath = @"E:\VS_Common\PanGuSegmentCore\examples\LucenePanGuDemo\ArticleIndex";
        static ArticleIndexHelper()
        {
            directory = FSDirectory.Open(new DirectoryInfo(DirectoryPath), NoLockFactory.GetNoLockFactory());
        }

        /// <summary>清理索引文件
        /// </summary>
        public static void ClearIndex()
        {
            using (IndexWriter writer = new IndexWriter(directory, new IndexWriterConfig(Lucene_Version, CreatePanGuAnalyzer())))
            {
                writer.DeleteAll();
                //Optimize??
                writer.Commit();
            }
        }

        /// <summary>创建索引
        /// </summary>
        public static void CreateIndex(Article article)
        {
            var analyzer = CreatePanGuAnalyzer();
            using (IndexWriter writer = new IndexWriter(directory, new IndexWriterConfig(Lucene_Version, analyzer)))
            {
                Document doc = new Document();
                doc.AddInt32Field("Id", article.Id, Field.Store.YES);
                doc.AddTextField("Title", article.Title, Field.Store.YES);
                doc.AddTextField("Content", article.Content, Field.Store.YES);
                doc.AddStringField("Author", article.Author, Field.Store.YES);
                writer.AddDocument(doc);
                writer.Commit();
            }
        }

        /// <summary>搜索
        /// </summary>
        public static List<Article> Search(string q)
        {
            IndexSearcher searcher = new IndexSearcher(DirectoryReader.Open(directory));

            //将输入的词汇进行分词
            Segment segment = new Segment();
            ICollection<WordInfo> words = segment.DoSegment(q, new PanGu.Match.MatchOptions()
            {
                ChineseNameIdentify = true,
                FilterStopWords = true,
                FrequencyFirst = true,
                EnglishMultiDimensionality = true,
                EnglishSegment = true
            }, new PanGu.Match.MatchParameter());
            PhraseQuery queryTitle = new PhraseQuery();
            PhraseQuery queryContent = new PhraseQuery();
            PhraseQuery queryAuthor = new PhraseQuery();

            foreach (string word in words.Select(x => x.Word))//先用空格，让用户去分词，空格分隔的就是词“计算机   专业”
            {
                queryTitle.Add(new Term("Title", word));  //标题
                queryContent.Add(new Term("Content", word));//内容
                                                            // queryAuthor.Add(new Term("Author", word));//作者
            }
            queryTitle.Slop = 100;//多个查询条件的词之间的最大距离.在文章中相隔太远 也就无意义.（例如 “大学生”这个查询条件和"简历"这个查询条件之间如果间隔的词太多也就没有意义了。）
            queryContent.Slop = 100;


            //TopScoreDocCollector是盛放查询结果的容器
            BooleanQuery query = new BooleanQuery(); //多条件查询  相当于或
            query.Add(queryTitle, Occur.SHOULD);
            query.Add(queryContent, Occur.SHOULD);

            //作者这个是固定的
            queryAuthor.Add(new Term("Author", q));
            query.Add(queryAuthor, Occur.SHOULD);
            TopScoreDocCollector collector = TopScoreDocCollector.Create(1000, true);
            searcher.Search(query, null, collector);//根据query查询条件进行查询，查询结果放入collector容器

            //TopDocs 指定0到GetTotalHits() 即所有查询结果中的文档 如果TopDocs(20,10)则意味着获取第20-30之间文档内容 达到分页的效果
            ScoreDoc[] docs = collector.GetTopDocs(0, collector.TotalHits).ScoreDocs;

            var articles = new List<Article>();
            for (int i = 0; i < docs.Length; i++)
            {
                int docId = docs[i].Doc;//得到查询结果文档的id（Lucene内部分配的id）
                Document doc = searcher.Doc(docId);//根据文档id来获得文档对象Document

                var article = new Article();

                article.Id = doc.GetField("Id").GetInt32ValueOrDefault();
                article.Title = doc.Get("Title");
                article.Content = doc.Get("Content");
                article.Author = doc.Get("Author");
                articles.Add(article);
            }
            return articles;
        }






        #region 创建PanGuAnalyzer
        public static PanGuAnalyzer CreatePanGuAnalyzer()
        {
            var analyzer = new PanGuAnalyzer(false, new PanGu.Match.MatchOptions()
            {
                ChineseNameIdentify = true,
                FilterStopWords = true,
                FrequencyFirst = true,
                EnglishMultiDimensionality = true,
                EnglishSegment = true,
                IgnoreCapital = true,
                MultiDimensionality = true
            }, new PanGu.Match.MatchParameter());
            return analyzer;
        }
        #endregion

    }
}
