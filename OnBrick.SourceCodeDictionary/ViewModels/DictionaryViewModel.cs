using Microsoft.Toolkit.Uwp;
using OnBrick.SourceCodeDictionary.Library;
using OnBrick.SourceCodeDictionary.Common;
using OnBrick.SourceCodeDictionary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.System;
using OnBrick.SourceCodeDictionary.Library.Models;

namespace OnBrick.SourceCodeDictionary.ViewModels
{
    public class DictionaryViewModel : BindableBase
    {
        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        public List<GridItemModel> GridItems { get; set; }
        //ResourceLoader _resourceLoader = ResourceLoader.GetForCurrentView();
        public List<DocumentModel> DocumentModels { get; set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => Set(ref _isLoading, value);
        }

        public DictionaryViewModel()
        {
            GridItems = new List<GridItemModel>();
        }

        public void Clear()
        {
            GridItems.Clear();
        }

        public async Task<List<ResultMessageModel>> Load(List<DocumentModel> documents)
        {
            List<ResultMessageModel> resultMessage = new List<ResultMessageModel>();
            await dispatcherQueue.EnqueueAsync(() => IsLoading = true);

            await Task.Delay(10);

            try
            {
                DocumentModels = documents;

                GridItems.Clear();
                int Sequence = 0;
                foreach (var document in documents)
                {
                    foreach (var c in document.Classes)
                    {
                        GridItems.Add(new GridItemModel
                        {
                            Sequence = Sequence++,

                            File = document.OriginalPath,
                            Namespace = c.Namespace,
                            FQDN = GetFQDN(c.Namespace, c.Identifier),
                            MemberType = MemberTypeEnum.Class.ToString(),

                            Public = c.Public,
                            Protected = c.Protected,
                            Private = c.Private,
                            Internal = c.Internal,
                            Partial = c.Partial,
                            Static = c.Static,
                            New = c.New,
                            Abstract = c.Abstract,
                            Override = c.Override,

                            Type = string.Empty,
                            ReturnType = string.Empty,
                            Identifier = c.Identifier,
                        });

                        foreach (var ctor in c.Constructs)
                        {
                            GridItems.Add(new GridItemModel
                            {
                                Sequence = Sequence++,

                                File = document.OriginalPath,
                                Namespace = c.Namespace,
                                FQDN = GetFQDN(c.Namespace, c.Identifier),
                                MemberType = MemberTypeEnum.Ctor.ToString(),

                                Public = ctor.Public,
                                Protected = ctor.Protected,
                                Private = ctor.Private,
                                Internal = ctor.Internal,
                                Partial = ctor.Partial,
                                Static = ctor.Static,
                                New = ctor.New,
                                Abstract = ctor.Abstract,
                                Override = ctor.Override,

                                Type = string.Empty,
                                ReturnType = string.Empty,
                                Identifier = ctor.Identifier,
                            });

                        }

                        if (c.Destruct != null)
                        {
                            var d = c.Destruct;

                            GridItems.Add(new GridItemModel
                            {
                                Sequence = Sequence++,

                                File = document.OriginalPath,
                                Namespace = c.Namespace,
                                FQDN = GetFQDN(c.Namespace, c.Identifier),
                                MemberType = MemberTypeEnum.Destructor.ToString(),

                                Public = string.Empty,
                                Protected = string.Empty,
                                Private = string.Empty,
                                Internal = string.Empty,
                                Partial = string.Empty,
                                Static = string.Empty,
                                New = string.Empty,
                                Abstract = string.Empty,
                                Override = string.Empty,

                                Type = string.Empty,
                                ReturnType = string.Empty,
                                Identifier = d.Identifier,
                            });
                        }


                        foreach (var f in c.Fields)
                        {
                            GridItems.Add(new GridItemModel
                            {
                                Sequence = Sequence++,

                                File = document.OriginalPath,
                                Namespace = c.Namespace,
                                FQDN = GetFQDN(c.Namespace, c.Identifier),
                                MemberType = MemberTypeEnum.Field.ToString(),

                                Public = f.Public,
                                Protected = f.Protected,
                                Private = f.Private,
                                Internal = f.Internal,
                                Partial = f.Partial,
                                Static = f.Static,
                                New = f.New,
                                Abstract = f.Abstract,
                                Override = f.Override,

                                Type = f.Type,
                                ReturnType = string.Empty,
                                Identifier = f.Identifier,
                            });
                        }

                        foreach (var prop in c.Properties)
                        {
                            GridItems.Add(new GridItemModel
                            {
                                Sequence = Sequence++,

                                File = document.OriginalPath,
                                Namespace = c.Namespace,
                                FQDN = GetFQDN(c.Namespace, c.Identifier),
                                MemberType = MemberTypeEnum.Property.ToString(),

                                Public = prop.Public,
                                Protected = prop.Protected,
                                Private = prop.Private,
                                Internal = prop.Internal,
                                Partial = prop.Partial,
                                Static = prop.Static,
                                New = prop.New,
                                Abstract = prop.Abstract,
                                Override = prop.Override,

                                Type = prop.Type,
                                ReturnType = string.Empty,
                                Identifier = prop.Identifier,
                            });
                        }

                        foreach (var m in c.Methods)
                        {
                            GridItems.Add(new GridItemModel
                            {
                                Sequence = Sequence++,

                                File = document.OriginalPath,
                                Namespace = c.Namespace,
                                FQDN = GetFQDN(c.Namespace, c.Identifier),
                                MemberType = MemberTypeEnum.Method.ToString(),

                                Public = m.Public,
                                Protected = m.Protected,
                                Private = m.Private,
                                Internal = m.Internal,
                                Partial = m.Partial,
                                Static = m.Static,
                                New = m.New,
                                Abstract = m.Abstract,
                                Override = m.Override,

                                Type = string.Empty,
                                ReturnType = m.ReturnType,
                                Identifier = m.Identifier,
                            });
                        }

                    }

                    foreach (var c in document.Interfaces)
                    {
                        GridItems.Add(new GridItemModel
                        {
                            Sequence = Sequence++,

                            File = document.OriginalPath,
                            Namespace = c.Namespace,
                            FQDN = GetFQDN(c.Namespace, c.Identifier),
                            MemberType = MemberTypeEnum.Interface.ToString(),

                            Public = c.Public,
                            Protected = c.Protected,
                            Private = c.Private,
                            Internal = c.Internal,
                            Partial = c.Partial,
                            Static = c.Static,
                            New = c.New,
                            Abstract = c.Abstract,
                            Override = c.Override,

                            Type = string.Empty,
                            ReturnType = string.Empty,
                            Identifier = c.Identifier,
                        });

                        foreach (var m in c.Methods)
                        {
                            GridItems.Add(new GridItemModel
                            {
                                Sequence = Sequence++,

                                File = document.OriginalPath,
                                Namespace = c.Namespace,
                                FQDN = GetFQDN(c.Namespace, c.Identifier),
                                MemberType = MemberTypeEnum.Method.ToString(),

                                Public = m.Public,
                                Protected = m.Protected,
                                Private = m.Private,
                                Internal = m.Internal,
                                Partial = m.Partial,
                                Static = m.Static,
                                New = m.New,
                                Abstract = m.Abstract,
                                Override = m.Override,

                                Type = string.Empty,
                                ReturnType = m.ReturnType,
                                Identifier = m.Identifier,
                            });
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                resultMessage.Add(new ResultMessageModel
                {
                    Category = ErrorTypeEnum.Others,
                    Message = ex.Message,
                    Position = "File",
                });
            }
            finally
            {
                await dispatcherQueue.EnqueueAsync(() => IsLoading = false);
            }

            return resultMessage;
        }

        private string GetFQDN(string @namespace, string identifier)
        {
            return string.Format("{0}.{1}", @namespace, identifier);
        }

    }
}