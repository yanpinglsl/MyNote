using System;
using System.Collections.Generic;
using System.IO;

namespace OfficeReportInterface
{
    public enum FolderAndFileType
    {
        ExcelType = 0,
        WordType = 1,
        FolderType = 5,
        InvalidType = 9,
    }

    /// <summary>
    /// TreeGirdView列表中的行的Tag
    /// </summary>
    public class TagForTemplate
    {
        /// <summary>
        /// 本地配置文件的项目
        /// </summary>
        public OfficeReportInfo officeReportInfo=new OfficeReportInfo();
        /// <summary>
        /// 文件或文件夹名称（仅名称）
        /// </summary>
        public string text=string.Empty;
        /// <summary>
        /// Tag代表类型
        /// </summary>
        public FolderAndFileType tagType=FolderAndFileType.ExcelType;
        /// <summary>
        /// 是否为根目录
        /// </summary>
        public bool isRootFolder=false;
        /// <summary>
        /// 是否为默认文件或文件夹
        /// </summary>
        public bool isDefault=false;
        /// <summary>
        /// 节点是否为叶子
        /// </summary>
        public bool leaf=false;
        /// <summary>
        /// 子节点
        /// </summary>
        public List<TagForTemplate> children=new List<TagForTemplate>();



        public bool ReadFromStream(BinaryReader br)
        {
            try
            {
                if (!officeReportInfo.ReadFromStream(br))
                    return false;
       
                text = br.ReadString();
                tagType = (FolderAndFileType)br.ReadInt32();
                isRootFolder = br.ReadBoolean();
                isDefault = br.ReadBoolean();
                leaf = br.ReadBoolean();
                int count = br.ReadInt32();
                if (children == null)
                    children = new List<TagForTemplate>();
                if (count > 0)
                {
                   
                    for (int i = 0; i < count; ++i)
                    {
                        TagForTemplate oneChild = new TagForTemplate();
                        if (!oneChild.ReadFromStream(br))
                            return false;
                     
                        if (children == null)
                            children = new List<TagForTemplate>();
                        children.Add(oneChild);
                    }
                }
               

                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }


        public bool WriteToStream( BinaryWriter bw)
        {
      
            try
            {
                if (!officeReportInfo.WriteToStream(bw))
                    return false;
                
                bw.Write(text);
                bw.Write((int)tagType);
                bw.Write(isRootFolder);
                bw.Write(isDefault);
                bw.Write(leaf);
                int count = 0;
                if (children != null)
                    count = children.Count;

            
                bw.Write(count);
                if (count > 0)
                {
                    for (int i = 0; i < count; ++i)
                    {
                        if (!children[i].WriteToStream(bw))
                            return false;
                       
                    }
                }
               
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public TagForTemplate()
        {
            officeReportInfo = new OfficeReportInfo();
            this.children = new List<TagForTemplate>();
        }
        public TagForTemplate(TagForTemplate node)
        {
            this.officeReportInfo = (OfficeReportInfo)node.officeReportInfo.Clone();
            this.text = node.text;
            this.tagType = node.tagType;
            this.isRootFolder = node.isRootFolder;
            this.isDefault = node.isDefault;
            this.children = node.children;
        }

      
    }
}
