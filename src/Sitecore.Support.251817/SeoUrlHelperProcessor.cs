namespace Sitecore.Support.XA.Foundation.ContentValidation.Pipelines.ItemProvider.CreateItem
{
  using System;
  using Sitecore.Caching;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Data.Templates;
  using Sitecore.Pipelines.ItemProvider.CreateItem;
  using Sitecore.XA.Foundation.SitecoreExtensions;
  using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;

  public class SeoUrlHelperProcessor : Sitecore.Pipelines.ItemProvider.CreateItem.CreateItemProcessor
  {
    public override void Process(CreateItemArgs args)
    {
      string normalizedName = args.ItemName.GetNormalizedName();
      Template destinationItemTemplate = TemplateManager.GetTemplate(args.TemplateId, args.Destination.Database);

      if (!JobsHelper.IsPublishing() && ShouldValidate(destinationItemTemplate) && !(args.ItemName == normalizedName) && !args.Destination.Paths.FullPath.StartsWith("/sitecore/templates/", StringComparison.OrdinalIgnoreCase))
      {
        Item destinationItem = args.FallbackProvider.CreateItem(normalizedName, args.Destination, args.TemplateId, args.NewId, args.SecurityCheck);
        HandleItemName(destinationItem, args.ItemName);
        args.Result = destinationItem;
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