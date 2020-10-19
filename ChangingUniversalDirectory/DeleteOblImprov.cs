using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DocsVision.Platform.ObjectModel;
using DocsVision.Platform.ObjectManager;
using DocsVision.Platform.ObjectManager.SearchModel;
using DocsVision.Platform.ObjectManager.Metadata;
using DocsVision.Platform.CardHost;
using DocsVision.TakeOffice.Cards.Constants;
using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.BackOffice.CardLib.CardDefs;

using SKB.Base;

namespace DeleteOblImprov
{
    public partial class DeleteOblImprov : Form
    {
        UserSession Session;
        ICardHost  Host;
        CardData UniversalCard;
        RowData DeviceType;
        CardData Document;
        RowData NewDictionary;

        public DeleteOblImprov(UserSession Session)
        {
            InitializeComponent();
            this.Session = Session;
            UniversalCard = Session.CardManager.GetDictionaryData(RefUniversal.ID);
        }

        private void CheckType_Click(object sender, EventArgs e)
        {
            //object activateParam = new object[] { "{DD20BF9B-90F8-4D9A-9553-5B5F17AD724E}", "", "", null, null, true };

            //Host = (ICardHost)CardHost.CreateInstance(Session);
            //DocsVision.Platform.CardHost.VbaCollection result = (DocsVision.Platform.CardHost.VbaCollection)Host.SelectFromCard(RefUniversal.ID, "Выберите тип записи универсального справочника", activateParam);
            if (this.TypeName.Text == "")
            {
                MessageBox.Show("Укажите тип прибора!");
            }
            else 
            {
                RowData ItemTypeSection = UniversalCard.Sections[UniversalCard.Type.AllSections["ItemType"].Id].Rows.First(s => s.GetString("Name") == "Приборы и комплектующие");
                DeviceType = ItemTypeSection.ChildSections[UniversalCard.Type.AllSections["Item"].Id].Rows.FirstOrDefault(s => s.GetString("Name") == this.TypeName.Text);
                if (DeviceType == null)
                { MessageBox.Show("Прибор с указанным названием не найден!"); }
                else
                {
                    this.TypeName.Enabled = false;
                }
                
            }
        }

        private void DeleteImprov_Click(object sender, EventArgs e)
        {
            if (this.TypeName.Text == "")
            {
                MessageBox.Show("Укажите тип прибора!");
                return;
            }

            if (this.DocName.Text == "")
            {
                MessageBox.Show("Укажите документ-основание!");
                return;
            }

            StatusStrip.Items["StatusText"].Text = "Процесс начат...";
            StatusStrip.Update();

            Guid DevicePassportTypeID = new Guid("{42826E25-AD0E-4D9C-8B18-CD88E6796972}");
            CardData CardTypeDictionary = Session.CardManager.GetDictionaryData(RefTypes.ID);
            SectionData DocumentTypes = CardTypeDictionary.Sections[RefTypes.DocumentTypes.ID];
            RowData DevicePassportType = DocumentTypes.GetRow(DevicePassportTypeID);
            RowDataCollection DevicePassportProperties = DevicePassportType.ChildSections[RefTypes.Properties.ID].Rows;

            SearchQuery searchQuery = Session.CreateSearchQuery();
            searchQuery.CombineResults = ConditionGroupOperation.And;

            CardTypeQuery typeQuery = searchQuery.AttributiveSearch.CardTypeQueries.AddNew(CardOrd.ID);
            SectionQuery sectionQuery = typeQuery.SectionQueries.AddNew(CardOrd.MainInfo.ID);
            sectionQuery.Operation = SectionQueryOperation.And;
            sectionQuery.ConditionGroup.Operation = ConditionGroupOperation.And;
            sectionQuery.ConditionGroup.Conditions.AddNew("Type", FieldType.RefId, ConditionOperation.Equals, DevicePassportTypeID);

            sectionQuery = typeQuery.SectionQueries.AddNew(CardOrd.Properties.ID);
            sectionQuery.Operation = SectionQueryOperation.And;
            sectionQuery.ConditionGroup.Operation = ConditionGroupOperation.And;
            sectionQuery.ConditionGroup.Conditions.AddNew("Name", FieldType.String, ConditionOperation.Equals, "Прибор");
            sectionQuery.ConditionGroup.Conditions.AddNew("Value", FieldType.RefId, ConditionOperation.Equals, DeviceType.Id);

            sectionQuery = typeQuery.SectionQueries.AddNew(CardOrd.SelectedValues.ID);
            sectionQuery.Operation = SectionQueryOperation.And;
            sectionQuery.ConditionGroup.Operation = ConditionGroupOperation.And;
            sectionQuery.ConditionGroup.Conditions.AddNew("SelectedValue", FieldType.RefCardId, ConditionOperation.Equals, Document.Id);

            // Получение текста запроса
            searchQuery.Limit = 0;
            string query = searchQuery.GetXml();
            Console.WriteLine(query);
            Console.ReadLine();
            CardDataCollection coll = Session.CardManager.FindCards(query);
            Clear();
 
            StatusStrip.Items["StatusText"].Text = "Найдено паспортов: " + coll.Count.ToString() + "...";
            StatusStrip.Update();

            int i = 0;
            for (i = 0; i < coll.Count; i++)
            {
                CardData Card = coll[i];
                Card.ForceUnlock();
                StatusStrip.Items["StatusText"].Text = i.ToString() + " из " + coll.Count + ". " + Card.Description;
                StatusStrip.Update();

                SectionData Properties = Card.Sections[CardOrd.Properties.ID];
                RowDataCollection DocumentCol = Properties.FindRow("@Name = 'Документ'").ChildSections[CardOrd.SelectedValues.ID].Rows;
                RowData DocumentRow = DocumentCol.First(r => new Guid(r.GetString("SelectedValue")).Equals(Document.Id));
                if (DocumentRow != null)
                { 
                    int Order = (int)DocumentRow.GetInt32("Order");

                    RowDataCollection Assembly = Properties.FindRow("@Name = 'Сборочный узел'").ChildSections[CardOrd.SelectedValues.ID].Rows;
                    RowDataCollection RepareItem = Properties.FindRow("@Name = 'Запись справочника ремонтных работ и доработок'").ChildSections[CardOrd.SelectedValues.ID].Rows;
                    RowDataCollection Indication = Properties.FindRow("@Name = 'Указание'").ChildSections[CardOrd.SelectedValues.ID].Rows;
                    RowDataCollection Check = Properties.FindRow("@Name = 'Выполнено'").ChildSections[CardOrd.SelectedValues.ID].Rows;
                    RowDataCollection CheckDate = Properties.FindRow("@Name = 'Дата выполнения'").ChildSections[CardOrd.SelectedValues.ID].Rows;
                    RowDataCollection Comments = Properties.FindRow("@Name = 'Комментарии'").ChildSections[CardOrd.SelectedValues.ID].Rows;

                    if (Assembly.FirstOrDefault(r => r.GetInt32("Order") == Order)!=null)
                        Assembly.Remove(Assembly.First(r => r.GetInt32("Order") == Order).Id);
                    if (RepareItem.FirstOrDefault(r => r.GetInt32("Order") == Order) != null)
                        RepareItem.Remove(RepareItem.First(r => r.GetInt32("Order") == Order).Id);
                    if (Indication.FirstOrDefault(r => r.GetInt32("Order") == Order) != null)
                        Indication.Remove(Indication.First(r => r.GetInt32("Order") == Order).Id);
                    if (Check.FirstOrDefault(r => r.GetInt32("Order") == Order) != null)
                        Check.Remove(Check.First(r => r.GetInt32("Order") == Order).Id);
                    if (CheckDate.FirstOrDefault(r => r.GetInt32("Order") == Order) != null)
                        CheckDate.Remove(CheckDate.First(r => r.GetInt32("Order") == Order).Id);
                    if (Comments.FirstOrDefault(r => r.GetInt32("Order") == Order) != null)
                        Comments.Remove(Comments.First(r => r.GetInt32("Order") == Order).Id);

                    DocumentCol.Remove(DocumentRow.Id);
                }
                Clear();
            }

            StatusStrip.Items["StatusText"].Text = i.ToString() + " карточек успешно обработано.";
            StatusStrip.Update();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void CheckDocument_Click(object sender, EventArgs e)
        {
            //
            //object activateParam = new object[] { "{E568490B-D73A-46BD-ADE1-54A0D9CA27D4}" };
            //Host = (ICardHost)CardHost.CreateInstance(Session);
            //Guid result = Host.SelectCard("Выберите докумнт-основание для доработки:", activateParam);

            if (this.DocName.Text == "")
            {
                MessageBox.Show("Укажите номер Разрешения!");
            }
            else
            {
                //Guid ResolutionTypeID = new Guid("{0051EE5E-C387-40DF-87D6-66160EB3395E}");
                //CardData CardTypeDictionary = Session.CardManager.GetDictionaryData(RefTypes.ID);
                //SectionData DocumentTypes = CardTypeDictionary.Sections[RefTypes.DocumentTypes.ID];
                //RowData ResolutionType = DocumentTypes.GetRow(ResolutionTypeID);
                //RowDataCollection ResolutionProperties = ResolutionType.ChildSections[new Guid("{752CCD78-111E-4F4C-9BB0-3933856EF44A}")].Rows;

                SearchQuery searchQuery = Session.CreateSearchQuery();
                searchQuery.CombineResults = ConditionGroupOperation.And;

                CardTypeQuery typeQuery = searchQuery.AttributiveSearch.CardTypeQueries.AddNew(new Guid("{0051EE5E-C387-40DF-87D6-66160EB3395E}"));
                SectionQuery sectionQuery = typeQuery.SectionQueries.AddNew(new Guid("{752CCD78-111E-4F4C-9BB0-3933856EF44A}"));
                sectionQuery.Operation = SectionQueryOperation.And;
                sectionQuery.ConditionGroup.Operation = ConditionGroupOperation.And;
                sectionQuery.ConditionGroup.Conditions.AddNew("Number", FieldType.String, ConditionOperation.Equals, this.DocName.Text);

                // Получение текста запроса
                searchQuery.Limit = 0;
                string query = searchQuery.GetXml();
                Console.WriteLine(query);
                Console.ReadLine();
                CardDataCollection coll = Session.CardManager.FindCards(query);
                Clear();


                if (coll.Count == 0)
                {
                    SearchQuery searchQuery2 = Session.CreateSearchQuery();
                    CardTypeQuery typeQuery2 = searchQuery2.AttributiveSearch.CardTypeQueries.AddNew(CardOrd.ID);
                    SectionQuery sectionQuery4 = typeQuery2.SectionQueries.AddNew(CardOrd.MainInfo.ID);
                    sectionQuery4.Operation = SectionQueryOperation.And;
                    sectionQuery4.ConditionGroup.Operation = ConditionGroupOperation.And;
                    sectionQuery4.ConditionGroup.Conditions.AddNew("Type", FieldType.RefId, ConditionOperation.Equals, new Guid("{66A8D7CC-5804-4237-9D13-B3897EF18CEC}"));

                    sectionQuery4 = typeQuery2.SectionQueries.AddNew(CardOrd.Properties.ID);
                    sectionQuery4.Operation = SectionQueryOperation.And;
                    sectionQuery4.ConditionGroup.Operation = ConditionGroupOperation.And;
                    sectionQuery4.ConditionGroup.Conditions.AddNew("Name", FieldType.String, ConditionOperation.Equals, "Номер разрешения");
                    sectionQuery4.ConditionGroup.Conditions.AddNew("Value", FieldType.String, ConditionOperation.Equals, this.DocName.Text);

                    
                    // Получение текста запроса
                    searchQuery2.Limit = 0;
                    string query2 = searchQuery2.GetXml();
                    Console.WriteLine(query2);
                    Console.ReadLine();
                    CardDataCollection coll2 = Session.CardManager.FindCards(query2);
                    Clear();
                    if (coll2.Count == 0)
                    {
                        MessageBox.Show("Разрешение с указанным номером не найдено!");
                    }
                    else
                    {
                        Document = coll2[0];
                        this.DocName.Text = Document.Description;
                        this.DocName.Enabled = false;
                    }
                }
                else
                {
                    Document = coll[0];
                    this.DocName.Text = Document.Description;
                    this.DocName.Enabled = false;
                }

            }
        }

        /// <summary>
        /// Освобождает занимаемую процессом память.
        /// </summary>
        private static void Clear()
        {
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
            Process p = Process.GetCurrentProcess();
            p.MinWorkingSet = p.MinWorkingSet;
        }
        /// <summary>
        /// Осуществляет обновление доработки в Паспорте прибора на основании данных Конструктора справочников
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BindImprov_Click(object sender, EventArgs e)
        {
            if (this.TypeName.Text == "")
            {
                MessageBox.Show("Укажите тип прибора!");
                return;
            }

            if (this.DocName.Text == "")
            {
                MessageBox.Show("Укажите документ-основание!");
                return;
            }

            StatusStrip.Items["StatusText"].Text = "Процесс начат...";
            StatusStrip.Update();

            // Получение записи справочника ремонтных работ и доработок
            //ObjectContext Context = Session.CreateContext();
            // Получение справочника "Ремонтные работы и доработки"
            //IBaseUniversalService baseUniversalService = Context.GetService<IBaseUniversalService>();
            //BaseUniversal baseUniversal = Context.GetObject<BaseUniversal>(RefBaseUniversal.ID);

            CardData baseUniversal = Session.CardManager.GetCardData(new Guid("{4538149D-1FC7-4D41-A104-890342C6B4F8}"));

            if (!baseUniversal.Sections[new Guid("{A1DCE6C1-DB96-4666-B418-5A075CDB02C9}")].GetAllRows().Any(r => r.Id == new Guid("{43A6DA44-899C-47D8-9567-2185E05D8524}")))
            {
                StatusStrip.Items["StatusText"].Text = "Ошибка! Не найден справочник ремонтных работ и доработок...";
                StatusStrip.Update();
                return;
            }

            // Поиск записей справочника
            SearchQuery searchQuery = Session.CreateSearchQuery();
            searchQuery.CombineResults = ConditionGroupOperation.And;

            CardTypeQuery typeQuery = searchQuery.AttributiveSearch.CardTypeQueries.AddNew(DocsVision.BackOffice.CardLib.CardDefs.CardBaseUniversalItem.ID);
            
            SectionQuery sectionQuery = typeQuery.SectionQueries.AddNew(DocsVision.BackOffice.CardLib.CardDefs.CardBaseUniversalItem.System.ID);
            sectionQuery.Operation = SectionQueryOperation.And;

            sectionQuery.ConditionGroup.Operation = ConditionGroupOperation.And;
            ConditionGroup ConditionGroup = sectionQuery.ConditionGroup.ConditionGroups.AddNew();
            ConditionGroup.Operation = ConditionGroupOperation.And;
            ConditionGroup.Conditions.AddNew(DocsVision.BackOffice.CardLib.CardDefs.CardBaseUniversalItem.System.Kind, FieldType.RefId, ConditionOperation.Equals, new Guid("{F4650B71-B131-41D2-AAFA-8DA1101ACA52}"));

            SectionQuery sectionQuery2 = typeQuery.SectionQueries.AddNew(new Guid("{3F9F3C1D-1CF1-4E71-BBE4-31D6AAD94EF7}"));
            sectionQuery2.Operation = SectionQueryOperation.And;

            sectionQuery2.ConditionGroup.Operation = ConditionGroupOperation.And;
            ConditionGroup ConditionGroup2 = sectionQuery2.ConditionGroup.ConditionGroups.AddNew();
            ConditionGroup2.Operation = ConditionGroupOperation.And;
            ConditionGroup2.Conditions.AddNew("BaseDocument", FieldType.RefId, ConditionOperation.Equals, Document.Id);
            ConditionGroup2.Conditions.AddNew("Status", FieldType.Int, ConditionOperation.Equals, 0);

            SectionQuery sectionQuery3 = typeQuery.SectionQueries.AddNew(new Guid("{E6DB53B7-7677-4978-8562-6B17917516A6}"));
            sectionQuery3.Operation = SectionQueryOperation.And;

            sectionQuery3.ConditionGroup.Operation = ConditionGroupOperation.And;
            ConditionGroup ConditionGroup3 = sectionQuery3.ConditionGroup.ConditionGroups.AddNew();
            ConditionGroup3.Operation = ConditionGroupOperation.And;
            ConditionGroup3.Conditions.AddNew("DeviceID", FieldType.RefId, ConditionOperation.Equals, DeviceType.Id);

            searchQuery.Limit = 0;
            string query = searchQuery.GetXml();
            CardDataCollection CardBaseUniversalItems = Session.CardManager.FindCards(query);
            CardData ItemCard = null;
            RowData ItemRow = null;
            if (CardBaseUniversalItems.Count() == 0)
            {
                StatusStrip.Items["StatusText"].Text = "Ошибка! Не найдена соответствующая карточка ремонтных работ и доработок...";
                StatusStrip.Update();
                return;
            }
            if (CardBaseUniversalItems.Count() > 1)
            {
                StatusStrip.Items["StatusText"].Text = "Ошибка! Найдено несколько подходящих карточек ремонтных работ и доработок...";
                StatusStrip.Update();
                return;
            }
            if (CardBaseUniversalItems.Count() == 1)
            {
                ItemCard = CardBaseUniversalItems.First();
                RowData ItemType = baseUniversal.Sections[new Guid("{A1DCE6C1-DB96-4666-B418-5A075CDB02C9}")].GetAllRows().First(r => r.Id == new Guid("{43A6DA44-899C-47D8-9567-2185E05D8524}"));
                ItemRow = ItemType.ChildSections[new Guid("{1B1A44FB-1FB1-4876-83AA-95AD38907E24}")].Rows.First(r => (Guid)r.GetGuid("ItemCard") == ItemCard.Id);
                if (ItemRow.IsNull())
                {
                    StatusStrip.Items["StatusText"].Text = "Ошибка! Не найдена соответствующая запись справочника ремонтных работ и доработок...";
                    StatusStrip.Update();
                    return;
                }
                else
                {
                    StatusStrip.Items["StatusText"].Text = "Запись найдена: " + ItemRow.GetString("Name") + "...";
                    StatusStrip.Update();
                }
            }


            Guid DevicePassportTypeID = new Guid("{42826E25-AD0E-4D9C-8B18-CD88E6796972}");
            CardData CardTypeDictionary = Session.CardManager.GetDictionaryData(RefTypes.ID);
            SectionData DocumentTypes = CardTypeDictionary.Sections[RefTypes.DocumentTypes.ID];
            RowData DevicePassportType = DocumentTypes.GetRow(DevicePassportTypeID);
            RowDataCollection DevicePassportProperties = DevicePassportType.ChildSections[RefTypes.Properties.ID].Rows;

            SearchQuery searchQuery2 = Session.CreateSearchQuery();
            searchQuery2.CombineResults = ConditionGroupOperation.And;

            CardTypeQuery typeQuery2 = searchQuery2.AttributiveSearch.CardTypeQueries.AddNew(CardOrd.ID);
            SectionQuery sectionQuery4 = typeQuery2.SectionQueries.AddNew(CardOrd.MainInfo.ID);
            sectionQuery4.Operation = SectionQueryOperation.And;
            sectionQuery4.ConditionGroup.Operation = ConditionGroupOperation.And;
            sectionQuery4.ConditionGroup.Conditions.AddNew("Type", FieldType.RefId, ConditionOperation.Equals, DevicePassportTypeID);

            sectionQuery4 = typeQuery2.SectionQueries.AddNew(CardOrd.Properties.ID);
            sectionQuery4.Operation = SectionQueryOperation.And;
            sectionQuery4.ConditionGroup.Operation = ConditionGroupOperation.And;
            sectionQuery4.ConditionGroup.Conditions.AddNew("Name", FieldType.String, ConditionOperation.Equals, "Прибор");
            sectionQuery4.ConditionGroup.Conditions.AddNew("Value", FieldType.RefId, ConditionOperation.Equals, DeviceType.Id);

            sectionQuery4 = typeQuery2.SectionQueries.AddNew(CardOrd.SelectedValues.ID);
            sectionQuery4.Operation = SectionQueryOperation.And;
            sectionQuery4.ConditionGroup.Operation = ConditionGroupOperation.And;
            sectionQuery4.ConditionGroup.Conditions.AddNew("SelectedValue", FieldType.RefCardId, ConditionOperation.Equals, Document.Id);

            // Получение текста запроса
            searchQuery2.Limit = 0;
            string query2 = searchQuery2.GetXml();
            Console.WriteLine(query2);
            Console.ReadLine();
            CardDataCollection coll = Session.CardManager.FindCards(query2);
            Clear();

            StatusStrip.Items["StatusText"].Text = "Найдено паспортов: " + coll.Count.ToString() + "...";
            StatusStrip.Update();

            int i = 0;
            for (i = 0; i < coll.Count; i++)
            {
                CardData Card = coll[i];
                Card.ForceUnlock();
                StatusStrip.Items["StatusText"].Text = i.ToString() + " из " + coll.Count + ". " + Card.Description;
                StatusStrip.Update();

                
                SectionData Properties = Card.Sections[CardOrd.Properties.ID];
                RowDataCollection DocumentCol = Properties.FindRow("@Name = 'Документ'").ChildSections[CardOrd.SelectedValues.ID].Rows;
                RowData DocumentRow = DocumentCol.First(r => new Guid(r.GetString("SelectedValue")).Equals(Document.Id));
                if (DocumentRow != null)
                {   
                    RowDataCollection Assembly = Properties.FindRow("@Name = 'Сборочный узел'").ChildSections[CardOrd.SelectedValues.ID].Rows;
                    RowDataCollection RepareItem = Properties.FindRow("@Name = 'Запись справочника ремонтных работ и доработок'").ChildSections[CardOrd.SelectedValues.ID].Rows;
                    RowDataCollection Indication = Properties.FindRow("@Name = 'Указание'").ChildSections[CardOrd.SelectedValues.ID].Rows;
                    RowDataCollection Check = Properties.FindRow("@Name = 'Выполнено'").ChildSections[CardOrd.SelectedValues.ID].Rows;
                    RowDataCollection CheckDate = Properties.FindRow("@Name = 'Дата выполнения'").ChildSections[CardOrd.SelectedValues.ID].Rows;
                    RowDataCollection Comments = Properties.FindRow("@Name = 'Комментарии'").ChildSections[CardOrd.SelectedValues.ID].Rows;

                    if (DocumentCol.Count != Assembly.Count)
                    {
                        foreach(RowData Doc in DocumentCol)
                        {
                            int Order = (int)Doc.GetInt32("Order");
                            if (Assembly.FirstOrDefault(r => r.GetInt32("Order") == Order).IsNull())
                            {
                                RowData NewRow = Assembly.AddNew();
                                NewRow.SetInt32("Order", Order);
                            }
                        }
                    }

                    if (DocumentCol.Count != RepareItem.Count)
                    {
                        foreach(RowData Doc in DocumentCol)
                        {
                            int Order = (int)Doc.GetInt32("Order");
                            if (RepareItem.FirstOrDefault(r => r.GetInt32("Order") == Order).IsNull())
                            {
                                RowData NewRow = RepareItem.AddNew();
                                NewRow.SetInt32("Order", Order);
                            }
                        }
                    }

                    if (DocumentCol.Count != Indication.Count)
                    {
                        foreach(RowData Doc in DocumentCol)
                        {
                            int Order = (int)Doc.GetInt32("Order");
                            if (Indication.FirstOrDefault(r => r.GetInt32("Order") == Order).IsNull())
                            {
                                RowData NewRow = Indication.AddNew();
                                NewRow.SetInt32("Order", Order);
                            }
                        }
                    }

                    if (DocumentCol.Count != Check.Count)
                    {
                        foreach(RowData Doc in DocumentCol)
                        {
                            int Order = (int)Doc.GetInt32("Order");
                            if (Check.FirstOrDefault(r => r.GetInt32("Order") == Order).IsNull())
                            {
                                RowData NewRow = Check.AddNew();
                                NewRow.SetInt32("Order", Order);
                            }
                        }
                    }

                    if (DocumentCol.Count != CheckDate.Count)
                    {
                        foreach(RowData Doc in DocumentCol)
                        {
                            int Order = (int)Doc.GetInt32("Order");
                            if (CheckDate.FirstOrDefault(r => r.GetInt32("Order") == Order).IsNull())
                            {
                                RowData NewRow = CheckDate.AddNew();
                                NewRow.SetInt32("Order", Order);
                            }
                        }
                    }

                    if (DocumentCol.Count != Comments.Count)
                    {
                        foreach(RowData Doc in DocumentCol)
                        {
                            int Order = (int)Doc.GetInt32("Order");
                            if (Comments.FirstOrDefault(r => r.GetInt32("Order") == Order).IsNull())
                            {
                                RowData NewRow = Comments.AddNew();
                                NewRow.SetInt32("Order", Order);
                            }
                        }
                    }

                    int CurrentOrder = (int)DocumentRow.GetInt32("Order");

                    RepareItem.First(r => r.GetInt32("Order") == CurrentOrder).SetGuid("SelectedValue", ItemRow.Id);

                    if (!ItemCard.Sections[new Guid("{3F9F3C1D-1CF1-4E71-BBE4-31D6AAD94EF7}")].FirstRow.GetGuid("AssemblyUnit").IsNull())
                            Assembly.First(r => r.GetInt32("Order") == CurrentOrder).SetGuid("SelectedValue", (Guid)ItemCard.Sections[new Guid("{3F9F3C1D-1CF1-4E71-BBE4-31D6AAD94EF7}")].FirstRow.GetGuid("AssemblyUnit"));
                    if (!ItemRow.GetString("Description").IsNull())
                            Indication.First(r => r.GetInt32("Order") == CurrentOrder).SetString("SelectedValue", ItemRow.GetString("Description"));
                }
                Clear();
            }

            StatusStrip.Items["StatusText"].Text = i.ToString() + " карточек успешно обработано.";
            StatusStrip.Update();
        }

        private void Next_Click(object sender, EventArgs e)
        {
            this.TypeName.Text = "";
            this.TypeName.Enabled = true;

            this.DocName.Text = "";
            this.DocName.Enabled = true;

            StatusStrip.Items["StatusText"].Text = "";
            StatusStrip.Update();
        }

        private void AddImprov_Click(object sender, EventArgs e)
        {
            RowData PartyRow = null;
            CardData UniversalDictionary = Session.CardManager.GetCardData(new Guid("{B2A438B7-8BB3-4B13-AF6E-F2F8996E148B}"));
            List<RowData> PartyItemRows = new List<RowData>();
            RowDataCollection PartyRowsCollection;
            if (!Document.Sections.Any(r=>r.Id == new Guid("{A8F93D97-496B-4675-B520-058B919146B7}")))
            {
                PartyRowsCollection = Document.Sections[new Guid("{5B6B407E-3D72-49E7-97D9-8E1E028C7274}")].Rows.First(r => r.GetString("Name") == "Партия").ChildSections[new Guid("{E6F5105F-8BD8-4500-9780-60D7C1402DDB}")].Rows;
                foreach (RowData CurrentRow in PartyRowsCollection)
                    PartyItemRows.Add(UniversalDictionary.GetItemRow((Guid)CurrentRow.GetGuid("SelectedValue")));
            }
            else
            {
                PartyRowsCollection = Document.Sections[new Guid("{A8F93D97-496B-4675-B520-058B919146B7}")].Rows;
                foreach (RowData CurrentRow in PartyRowsCollection)
                    PartyItemRows.Add(UniversalDictionary.GetItemRow((Guid)CurrentRow.GetGuid("PartyId")));
            }
                
                
                //List<RowData> PartyItemRows = PartyRowsCollection.Select(r => UniversalDictionary.GetItemRow((Guid)r.GetGuid("Id"))).ToList();

                if (PartyItemRows.Any(r => UniversalDictionary.GetItemPropertyValue(r.Id, "Наименование прибора").ToGuid() == DeviceType.Id))
                {
                    PartyRow = PartyItemRows.FirstOrDefault(r => UniversalDictionary.GetItemPropertyValue(r.Id, "Наименование прибора").ToGuid() == DeviceType.Id);
                }
                else
                {
                    StatusStrip.Items["StatusText"].Text = "Ошибка! В Разрешении не указана партия для текущего прибора.";
                    StatusStrip.Update();
                    switch (MessageBox.Show("Внести доработку во все приборы в эксплуатации?", "Ошибка", MessageBoxButtons.YesNo))
                    {
                        case DialogResult.Yes:
                            StatusStrip.Items["StatusText"].Text = "Продолжение работы...";
                            StatusStrip.Update();
                            break;
                        case DialogResult.No:
                            return;
                            break;
                    }
                }
                List<Guid> ImprovPartiesGuid = new List<Guid>();
                if (!PartyRow.IsNull())
                {
                    RowData PartyDicType = UniversalDictionary.Sections[new Guid("{5E3ED23A-2B5E-47F2-887C-E154ACEAFB97}")].Rows.First(r => r.GetString("Name") == "Справочник партий");
                    IEnumerable<RowData> AllParties = PartyDicType.ChildSections[new Guid("{DD20BF9B-90F8-4D9A-9553-5B5F17AD724E}")].Rows.Where(r => (UniversalDictionary.GetItemPropertyValue(r.Id, "Наименование прибора").ToGuid() == DeviceType.Id) && (!UniversalDictionary.GetItemPropertyValue(r.Id, "Год начала выпуска").IsNull()));
                    PartyComparer PC = new PartyComparer();
                    List<RowData> ImprovParties = AllParties.Where(r => PC.Compare(r.GetString("Name"), PartyRow.GetString("Name")) == 1).ToList();
                    foreach (RowData CurrentParty in ImprovParties)
                        ImprovPartiesGuid.Add(CurrentParty.Id);
                }
            
            

            CardData baseUniversal = Session.CardManager.GetCardData(new Guid("{4538149D-1FC7-4D41-A104-890342C6B4F8}"));

            if (!baseUniversal.Sections[new Guid("{A1DCE6C1-DB96-4666-B418-5A075CDB02C9}")].GetAllRows().Any(r => r.Id == new Guid("{43A6DA44-899C-47D8-9567-2185E05D8524}")))
            {
                StatusStrip.Items["StatusText"].Text = "Ошибка! Не найден справочник ремонтных работ и доработок...";
                StatusStrip.Update();
                return;
            }

            // Поиск записей справочника
            SearchQuery searchQuery = Session.CreateSearchQuery();
            searchQuery.CombineResults = ConditionGroupOperation.And;

            CardTypeQuery typeQuery = searchQuery.AttributiveSearch.CardTypeQueries.AddNew(DocsVision.BackOffice.CardLib.CardDefs.CardBaseUniversalItem.ID);
            
            SectionQuery sectionQuery = typeQuery.SectionQueries.AddNew(DocsVision.BackOffice.CardLib.CardDefs.CardBaseUniversalItem.System.ID);
            sectionQuery.Operation = SectionQueryOperation.And;

            sectionQuery.ConditionGroup.Operation = ConditionGroupOperation.And;
            ConditionGroup ConditionGroup = sectionQuery.ConditionGroup.ConditionGroups.AddNew();
            ConditionGroup.Operation = ConditionGroupOperation.And;
            ConditionGroup.Conditions.AddNew(DocsVision.BackOffice.CardLib.CardDefs.CardBaseUniversalItem.System.Kind, FieldType.RefId, ConditionOperation.Equals, new Guid("{F4650B71-B131-41D2-AAFA-8DA1101ACA52}"));

            SectionQuery sectionQuery2 = typeQuery.SectionQueries.AddNew(new Guid("{3F9F3C1D-1CF1-4E71-BBE4-31D6AAD94EF7}"));
            sectionQuery2.Operation = SectionQueryOperation.And;

            sectionQuery2.ConditionGroup.Operation = ConditionGroupOperation.And;
            ConditionGroup ConditionGroup2 = sectionQuery2.ConditionGroup.ConditionGroups.AddNew();
            ConditionGroup2.Operation = ConditionGroupOperation.And;
            ConditionGroup2.Conditions.AddNew("BaseDocument", FieldType.RefId, ConditionOperation.Equals, Document.Id);
            ConditionGroup2.Conditions.AddNew("Status", FieldType.Int, ConditionOperation.Equals, 0);

            SectionQuery sectionQuery3 = typeQuery.SectionQueries.AddNew(new Guid("{E6DB53B7-7677-4978-8562-6B17917516A6}"));
            sectionQuery3.Operation = SectionQueryOperation.And;

            sectionQuery3.ConditionGroup.Operation = ConditionGroupOperation.And;
            ConditionGroup ConditionGroup3 = sectionQuery3.ConditionGroup.ConditionGroups.AddNew();
            ConditionGroup3.Operation = ConditionGroupOperation.And;
            ConditionGroup3.Conditions.AddNew("DeviceID", FieldType.RefId, ConditionOperation.Equals, DeviceType.Id);

            searchQuery.Limit = 0;
            string query = searchQuery.GetXml();
            CardDataCollection CardBaseUniversalItems = Session.CardManager.FindCards(query);
            CardData ItemCard = null;
            RowData ItemRow = null;
            if (CardBaseUniversalItems.Count() == 0)
            {
                StatusStrip.Items["StatusText"].Text = "Ошибка! Не найдена соответствующая карточка ремонтных работ и доработок...";
                StatusStrip.Update();
                return;
            }
            if (CardBaseUniversalItems.Count() > 1)
            {
                StatusStrip.Items["StatusText"].Text = "Ошибка! Найдено несколько подходящих карточек ремонтных работ и доработок...";
                StatusStrip.Update();
                return;
            }
            if (CardBaseUniversalItems.Count() == 1)
            {
                ItemCard = CardBaseUniversalItems.First();
                RowData ItemType = baseUniversal.Sections[new Guid("{A1DCE6C1-DB96-4666-B418-5A075CDB02C9}")].GetAllRows().First(r => r.Id == new Guid("{43A6DA44-899C-47D8-9567-2185E05D8524}"));
                ItemRow = ItemType.ChildSections[new Guid("{1B1A44FB-1FB1-4876-83AA-95AD38907E24}")].Rows.First(r => (Guid)r.GetGuid("ItemCard") == ItemCard.Id);
                if (ItemRow.IsNull())
                {
                    StatusStrip.Items["StatusText"].Text = "Ошибка! Не найдена соответствующая запись справочника ремонтных работ и доработок...";
                    StatusStrip.Update();
                    return;
                }
                else
                {
                    StatusStrip.Items["StatusText"].Text = "Запись найдена: " + ItemRow.GetString("Name") + "...";
                    StatusStrip.Update();
                }
            }
          
            Guid DevicePassportTypeID = new Guid("{42826E25-AD0E-4D9C-8B18-CD88E6796972}");
            CardData CardTypeDictionary = Session.CardManager.GetDictionaryData(RefTypes.ID);
            SectionData DocumentTypes = CardTypeDictionary.Sections[RefTypes.DocumentTypes.ID];
            RowData DevicePassportType = DocumentTypes.GetRow(DevicePassportTypeID);
            RowDataCollection DevicePassportProperties = DevicePassportType.ChildSections[RefTypes.Properties.ID].Rows;

            SearchQuery searchQuery2 = Session.CreateSearchQuery();
            searchQuery2.CombineResults = ConditionGroupOperation.And;

            CardTypeQuery typeQuery2 = searchQuery2.AttributiveSearch.CardTypeQueries.AddNew(CardOrd.ID);
            SectionQuery sectionQuery4 = typeQuery2.SectionQueries.AddNew(CardOrd.MainInfo.ID);
            sectionQuery4.Operation = SectionQueryOperation.And;
            sectionQuery4.ConditionGroup.Operation = ConditionGroupOperation.And;
            sectionQuery4.ConditionGroup.Conditions.AddNew("Type", FieldType.RefId, ConditionOperation.Equals, DevicePassportTypeID);

            sectionQuery4 = typeQuery2.SectionQueries.AddNew(CardOrd.Properties.ID);
            sectionQuery4.Operation = SectionQueryOperation.And;
            sectionQuery4.ConditionGroup.Operation = ConditionGroupOperation.And;
            sectionQuery4.ConditionGroup.Conditions.AddNew("Name", FieldType.String, ConditionOperation.Equals, "Прибор");
            sectionQuery4.ConditionGroup.Conditions.AddNew("Value", FieldType.RefId, ConditionOperation.Equals, DeviceType.Id);

            if (ImprovPartiesGuid.Count != 0)
            {
                sectionQuery4 = typeQuery2.SectionQueries.AddNew(CardOrd.Properties.ID);
                sectionQuery4.Operation = SectionQueryOperation.And;
                sectionQuery4.ConditionGroup.Operation = ConditionGroupOperation.And;
                sectionQuery4.ConditionGroup.Conditions.AddNew("Name", FieldType.String, ConditionOperation.Equals, "№ партии");
                ConditionGroup PartyGroup = sectionQuery4.ConditionGroup.ConditionGroups.AddNew();
                PartyGroup.Operation = ConditionGroupOperation.Or;
                for (int j = 0; j < ImprovPartiesGuid.Count; j++)
                {
                    PartyGroup.Conditions.AddNew("Value", FieldType.RefId, ConditionOperation.OneOf, ImprovPartiesGuid.ToArray()[j]);
                }
            }
            else
            {
                sectionQuery4 = typeQuery2.SectionQueries.AddNew(CardOrd.Properties.ID);
                sectionQuery4.Operation = SectionQueryOperation.And;
                sectionQuery4.ConditionGroup.Operation = ConditionGroupOperation.And;
                sectionQuery4.ConditionGroup.Conditions.AddNew("Name", FieldType.String, ConditionOperation.Equals, "Состояние прибора");
                sectionQuery4.ConditionGroup.Conditions.AddNew("Value", FieldType.RefId, ConditionOperation.OneOf, new int[] {5,6});
            }
            // Получение текста запроса
            searchQuery2.Limit = 0;
            string query2 = searchQuery2.GetXml();
            Console.WriteLine(query2);
            Console.ReadLine();
            CardDataCollection coll = Session.CardManager.FindCards(query2);
            Clear();

            StatusStrip.Items["StatusText"].Text = "Найдено паспортов: " + coll.Count.ToString() + "...";
            StatusStrip.Update();

            int i = 0;
            for (i = 0; i < coll.Count; i++)
            {
                CardData Card = coll[i];
                Card.ForceUnlock();
                StatusStrip.Items["StatusText"].Text = i.ToString() + " из " + coll.Count + ". " + Card.Description;
                StatusStrip.Update();

                SectionData Properties = Card.Sections[CardOrd.Properties.ID];
                RowDataCollection DocumentCol = Properties.FindRow("@Name = 'Документ'").ChildSections[CardOrd.SelectedValues.ID].Rows;
                RowDataCollection Assembly = Properties.FindRow("@Name = 'Сборочный узел'").ChildSections[CardOrd.SelectedValues.ID].Rows;
                RowDataCollection RepareItem = Properties.FindRow("@Name = 'Запись справочника ремонтных работ и доработок'").ChildSections[CardOrd.SelectedValues.ID].Rows;
                RowDataCollection Indication = Properties.FindRow("@Name = 'Указание'").ChildSections[CardOrd.SelectedValues.ID].Rows;
                RowDataCollection Check = Properties.FindRow("@Name = 'Выполнено'").ChildSections[CardOrd.SelectedValues.ID].Rows;
                RowDataCollection CheckDate = Properties.FindRow("@Name = 'Дата выполнения'").ChildSections[CardOrd.SelectedValues.ID].Rows;
                RowDataCollection Comments = Properties.FindRow("@Name = 'Комментарии'").ChildSections[CardOrd.SelectedValues.ID].Rows;

                if (DocumentCol.Count != Assembly.Count)
                {
                    foreach (RowData Doc in DocumentCol)
                    {
                        int Order = (int)Doc.GetInt32("Order");
                        if (Assembly.FirstOrDefault(r => r.GetInt32("Order") == Order).IsNull())
                        {
                            RowData NewRow = Assembly.AddNew();
                            NewRow.SetInt32("Order", Order);
                        }
                    }
                }

                if (DocumentCol.Count != RepareItem.Count)
                {
                    foreach (RowData Doc in DocumentCol)
                    {
                        int Order = (int)Doc.GetInt32("Order");
                        if (RepareItem.FirstOrDefault(r => r.GetInt32("Order") == Order).IsNull())
                        {
                            RowData NewRow = RepareItem.AddNew();
                            NewRow.SetInt32("Order", Order);
                        }
                    }
                }

                if (DocumentCol.Count != Indication.Count)
                {
                    foreach (RowData Doc in DocumentCol)
                    {
                        int Order = (int)Doc.GetInt32("Order");
                        if (Indication.FirstOrDefault(r => r.GetInt32("Order") == Order).IsNull())
                        {
                            RowData NewRow = Indication.AddNew();
                            NewRow.SetInt32("Order", Order);
                        }
                    }
                }

                if (DocumentCol.Count != Check.Count)
                {
                    foreach (RowData Doc in DocumentCol)
                    {
                        int Order = (int)Doc.GetInt32("Order");
                        if (Check.FirstOrDefault(r => r.GetInt32("Order") == Order).IsNull())
                        {
                            RowData NewRow = Check.AddNew();
                            NewRow.SetInt32("Order", Order);
                        }
                    }
                }

                if (DocumentCol.Count != CheckDate.Count)
                {
                    foreach (RowData Doc in DocumentCol)
                    {
                        int Order = (int)Doc.GetInt32("Order");
                        if (CheckDate.FirstOrDefault(r => r.GetInt32("Order") == Order).IsNull())
                        {
                            RowData NewRow = CheckDate.AddNew();
                            NewRow.SetInt32("Order", Order);
                        }
                    }
                }

                if (DocumentCol.Count != Comments.Count)
                {
                    foreach (RowData Doc in DocumentCol)
                    {
                        int Order = (int)Doc.GetInt32("Order");
                        if (Comments.FirstOrDefault(r => r.GetInt32("Order") == Order).IsNull())
                        {
                            RowData NewRow = Comments.AddNew();
                            NewRow.SetInt32("Order", Order);
                        }
                    }
                }

                if (!DocumentCol.Any(r => r.GetGuid("SelectedValue") == Document.Id))
                {
                    int CurrentOrder = DocumentCol.Count + 1;
                    RowData NewRow2 = DocumentCol.AddNew();
                    NewRow2.SetInt32("Order", CurrentOrder);
                    NewRow2.SetGuid("SelectedValue", Document.Id);

                    NewRow2 = RepareItem.AddNew();
                    NewRow2.SetInt32("Order", CurrentOrder);
                    NewRow2.SetGuid("SelectedValue", ItemRow.Id);

                    NewRow2 = Assembly.AddNew();
                    NewRow2.SetInt32("Order", CurrentOrder);
                    if (!ItemCard.Sections[new Guid("{3F9F3C1D-1CF1-4E71-BBE4-31D6AAD94EF7}")].FirstRow.GetGuid("AssemblyUnit").IsNull())
                        NewRow2.SetGuid("SelectedValue", (Guid)ItemCard.Sections[new Guid("{3F9F3C1D-1CF1-4E71-BBE4-31D6AAD94EF7}")].FirstRow.GetGuid("AssemblyUnit"));

                    NewRow2 = Indication.AddNew();
                    NewRow2.SetInt32("Order", CurrentOrder);
                    if (!ItemCard.Sections[new Guid("{3F9F3C1D-1CF1-4E71-BBE4-31D6AAD94EF7}")].FirstRow.GetGuid("Instructions").IsNull())
                        NewRow2.SetGuid("SelectedValue", (Guid)ItemCard.Sections[new Guid("{3F9F3C1D-1CF1-4E71-BBE4-31D6AAD94EF7}")].FirstRow.GetGuid("Instructions"));

                    NewRow2 = Check.AddNew();
                    NewRow2.SetInt32("Order", CurrentOrder);

                    NewRow2 = CheckDate.AddNew();
                    NewRow2.SetInt32("Order", CurrentOrder);

                    NewRow2 = Comments.AddNew();
                    NewRow2.SetInt32("Order", CurrentOrder);
                }
                Clear();
            }

            StatusStrip.Items["StatusText"].Text = i.ToString() + " карточек успешно обработано.";
            StatusStrip.Update();
        }
    }
    /// <summary>
    /// Сравнение партий по месяцу и году выпуска
    /// </summary>
    public class PartyComparer : IComparer<string>
    {
        /// <summary>
        /// Компаратор
        /// </summary>
        /// <param name="party1">Партия 1</param>
        /// <param name="party2">Партия 2</param>
        /// <returns></returns>
        public int Compare(string party1, string party2)
        {
            string[] PartyOptions1 = party1.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
            string[] PartyOptions2 = party2.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);

            int PartyYear1 = Convert.ToInt32(PartyOptions1[2].Split('/')[1]); ;
            int PartyMonth1 = Convert.ToInt32(PartyOptions1[2].Split('/')[0]); ;

            int PartyYear2 = Convert.ToInt32(PartyOptions2[2].Split('/')[1]); ;
            int PartyMonth2 = Convert.ToInt32(PartyOptions2[2].Split('/')[0]); ;

            if (PartyYear1 > PartyYear2)
                return -1;
            else
            {
                if (PartyYear1 == PartyYear2)
                {
                    if (PartyMonth1 > PartyMonth2)
                        return -1;
                    else if (PartyMonth1 == PartyMonth2)
                        return 0;
                    else
                        return 1;
                }
                else
                    return 1;
            }
        }
    }
}
