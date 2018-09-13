namespace Sitecore.Support.XA.Foundation.ContentValidation.Pipelines.ItemProvider.CreateItem
{
  using System;
  using Sitecore.Caching;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Data.Templates;
  using Sitecore.Pipelines;
  using Sitecore.Pipelines.ItemProvider.CreateItem;
  using Sitecore.XA.Foundation.SitecoreExtensions;
  using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;

  public class SeoUrlHelperProcessor
  { 
    public  void Process(ProviderResultPipelineArgs<Item, ItemProviderBase> args)
    {
      if (!JobsHelper.IsPublishing())
      {
        CreateItemArgs createItemArgs = args as CreateItemArgs;
        if (args != null)
        {
          string normalizedName = createItemArgs.ItemName.GetNormalizedName();
          Template destinationItemTemplate = TemplateManager.GetTemplate(createItemArgs.TemplateId, createItemArgs.Destination.Database);

          if (ShouldValidate(destinationItemTemplate) && !(createItemArgs.ItemName == normalizedName) && !createItemArgs.Destination.Paths.FullPath.StartsWith("/sitecore/templates/", StringComparison.OrdinalIgnoreCase))
          {
            Item destinationItem = args.FallbackProvider.CreateItem(normalizedName, createItemArgs.Destination, createItemArgs.TemplateId, createItemArgs.NewId, createItemArgs.SecurityCheck);
            HandleItemName(destinationItem, createItemArgs.ItemName);
            args.Result = destinationItem;
          }
        }
      }
    }

    protected virtual void HandleItemName(Item destinationItem, string originalName)
    {
      ItemPathsCache itemPathsCache = CacheManager.GetItemPathsCache(destinationItem.Database);
      destinationItem.Editing.BeginEdit();
      destinationItem.Appearance.DisplayName = originalName;
      destinationItem.Editing.EndEdit();
      itemPathsCache.InvalidateCache(destinationItem);
    }

    protected virtual bool ShouldValidate(Template item)
    {
      return item.InheritsFrom(Sitecore.XA.Foundation.ContentValidation.Templates._SeoUrlValid.ID);
    }
  }
}