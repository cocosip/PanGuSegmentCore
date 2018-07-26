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

namespace PanGuLuceneDemo
{
    /// <summary>商品索引帮助类
    /// </summary>
    public class ProductIndexHelper
    {
        private static LuceneVersion Lucene_Version = LuceneVersion.LUCENE_48;
        private static Lucene.Net.Store.Directory directory;
        //索引存放目录
        private static string DirectoryPath = @"E:\VS_Common\PanGuSegmentCore\examples\PanGuLuceneDemo\ProductIndex";
        static ProductIndexHelper()
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
        public static void CreateIndex(Product product)
        {
            using (IndexWriter writer = new IndexWriter(directory, new IndexWriterConfig(Lucene_Version, CreatePanGuAnalyzer())))
            {
                Document doc = new Document();
                doc.AddStringField("Id", product.Id.ToString(), Field.Store.YES);
                doc.AddTextField("Name", product.Name, Field.Store.YES);
                doc.AddTextField("PrefixName", product.PrefixName ?? product.Name, Field.Store.YES);
                doc.AddTextField("SuffixName", product.SuffixName ?? product.Name, Field.Store.YES);
                doc.AddStringField("ProductCode", product.ProductCode, Field.Store.YES);
                doc.AddStringField("CategoryName", product.CategoryName, Field.Store.YES);
                doc.AddStringField("BrandName", product.BrandName, Field.Store.YES);
                writer.AddDocument(doc);
                writer.Commit();
            }
        }

        /// <summary>创建索引
        /// </summary>
        public static void BatchCreateIndex(List<Product> products)
        {
            using (IndexWriter writer = new IndexWriter(directory, new IndexWriterConfig(Lucene_Version, CreatePanGuAnalyzer())))
            {
                //var docs = new List<Document>();
                foreach (var product in products)
                {
                    Document doc = new Document();
                    doc.AddStringField("Id", product.Id.ToString(), Field.Store.YES);
                    doc.AddTextField("Name", product.Name, Field.Store.YES);
                    doc.AddTextField("PrefixName", product.PrefixName ?? product.Name, Field.Store.YES);
                    doc.AddTextField("SuffixName", product.SuffixName ?? product.Name, Field.Store.YES);
                    doc.AddStringField("ProductCode", product.ProductCode, Field.Store.YES);
                    doc.AddStringField("CategoryName", product.CategoryName, Field.Store.YES);
                    doc.AddStringField("BrandName", product.BrandName, Field.Store.YES);
                    //docs.Add(doc);
                    writer.AddDocument(doc);
                    //writer.Flush(true, false);
                    //writer.Flush(true, true);
                    //writer.Commit();

                    writer.Flush(true, true);
                }
                //var a = writer.CommitData;
                // writer.Commit();
                //writer.AddDocuments(docs, CreatePanGuAnalyzer());
                //writer.Commit();
            }
        }


        /// <summary>更新索引
        /// </summary>
        public static void UpdateIndex(Product product)
        {
            using (IndexWriter writer = new IndexWriter(directory, new IndexWriterConfig(Lucene_Version, CreatePanGuAnalyzer())))
            {
                Document doc = new Document();
                doc.AddStringField("Id", product.Id.ToString(), Field.Store.YES);
                doc.AddTextField("Name", product.Name, Field.Store.YES);
                doc.AddTextField("PrefixName", product.PrefixName ?? product.Name, Field.Store.YES);
                doc.AddTextField("SuffixName", product.SuffixName ?? product.Name, Field.Store.YES);
                doc.AddStringField("ProductCode", product.ProductCode, Field.Store.YES);
                doc.AddStringField("CategoryName", product.CategoryName, Field.Store.YES);
                doc.AddStringField("BrandName", product.BrandName, Field.Store.YES);
                writer.UpdateDocument(new Term("Id", product.Id.ToString()), doc);
                //Optimize??
                writer.Commit();
            }
        }

        /// <summary>删除商品
        /// </summary>
        public static void DeleteIndex(Product product)
        {
            using (IndexWriter writer = new IndexWriter(directory, new IndexWriterConfig(Lucene_Version, CreatePanGuAnalyzer())))
            {
                writer.DeleteDocuments(new Term("Id", product.Id.ToString()));
                writer.Commit();
            }
        }

        /// <summary>搜索
        /// </summary>
        public static List<Product> Search(string q)
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
            PhraseQuery queryName = new PhraseQuery();
            PhraseQuery queryPrefixName = new PhraseQuery();
            PhraseQuery querySuffixName = new PhraseQuery();

            foreach (string word in words.Select(x => x.Word))//先用空格，让用户去分词，空格分隔的就是词“计算机   专业”
            {
                queryName.Add(new Term("Name", word));  //商品名
                queryPrefixName.Add(new Term("PrefixName", word));//前缀
                querySuffixName.Add(new Term("SuffixName", word));//后缀
            }
            queryName.Slop = 100;//多个查询条件的词之间的最大距离.在文章中相隔太远 也就无意义.（例如 “大学生”这个查询条件和"简历"这个查询条件之间如果间隔的词太多也就没有意义了。）
            queryPrefixName.Slop = 100;
            querySuffixName.Slop = 100;

            //TopScoreDocCollector是盛放查询结果的容器
            BooleanQuery query = new BooleanQuery(); //多条件查询  相当于或
            query.Add(queryName, Occur.SHOULD);
            query.Add(queryPrefixName, Occur.SHOULD);
            query.Add(querySuffixName, Occur.SHOULD);



            //编号
            PhraseQuery queryProductCode = new PhraseQuery();
            queryProductCode.Add(new Term("ProductCode", q));
            query.Add(queryProductCode, Occur.SHOULD);
            //类目名
            PhraseQuery queryCategoryName = new PhraseQuery();
            queryCategoryName.Add(new Term("CategoryName", q));
            query.Add(queryCategoryName, Occur.SHOULD);
            //品牌名
            PhraseQuery queryBrandName = new PhraseQuery();
            queryBrandName.Add(new Term("BrandName", q));
            query.Add(queryBrandName, Occur.SHOULD);


            TopScoreDocCollector collector = TopScoreDocCollector.Create(1000, true);
            searcher.Search(query, null, collector);//根据query查询条件进行查询，查询结果放入collector容器

            //TopDocs 指定0到GetTotalHits() 即所有查询结果中的文档 如果TopDocs(20,10)则意味着获取第20-30之间文档内容 达到分页的效果
            ScoreDoc[] docs = collector.GetTopDocs(0, collector.TotalHits).ScoreDocs;
            //如果是分页,需要用下面的代码
            ////获取上一页的最后一个元素  
            //ScoreDoc lastSd = GetLastScoreDoc(pageIndex, pageSize, query, searcher);
            ////通过最后一个元素去搜索下一页的元素  
            //ScoreDoc[] docs = searcher.SearchAfter(lastSd, query, pageSize).ScoreDocs;

            var products = new List<Product>();
            for (int i = 0; i < docs.Length; i++)
            {
                int docId = docs[i].Doc;//得到查询结果文档的id（Lucene内部分配的id）
                Document doc = searcher.Doc(docId);//根据文档id来获得文档对象Document

                var product = new Product();

                product.Id = Convert.ToInt32(doc.Get("Id"));
                product.Name = doc.Get("Name");
                product.PrefixName = doc.Get("PrefixName");
                product.SuffixName = doc.Get("SuffixName");
                product.ProductCode = doc.Get("ProductCode");
                product.CategoryName = doc.Get("CategoryName");
                product.BrandName = doc.Get("BrandName");
                products.Add(product);
            }
            return products;
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

        /// <summary>根据页码和分页大小获取上一次的最后一个ScoreDoc
        /// </summary>
        private ScoreDoc GetLastScoreDoc(int pageIndex, int pageSize, Query query, IndexSearcher searcher)
        {
            if (pageIndex == 1)
            {
                return null;
            }
            int num = pageSize * (pageIndex - 1);//获取上一页的最后数量  
            TopDocs tds = searcher.Search(query, num);
            return tds.ScoreDocs[num - 1];
        }
        #endregion
    }
}
