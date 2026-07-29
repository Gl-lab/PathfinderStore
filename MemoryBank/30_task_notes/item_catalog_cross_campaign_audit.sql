-- Аудит cross-campaign ссылок перед деплоем единого правила видимости конфигураций
-- (см. item_catalog_admin_spec.md, раздел «Единое правило видимости»).
-- Обе выборки должны быть пустыми. Непустые строки лечатся созданием одноимённой
-- конфигурации своей кампании (POST /api/item-catalog-admin/campaigns/{id}/configurations)
-- и перепривязкой предложения/экземпляра.

-- 1. Предложения магазинов, ссылающиеся на конфигурации чужих кампаний.
SELECT
    offer."Id"           AS offer_id,
    offer."CampaignId"   AS offer_campaign_id,
    offer."ShopId"       AS shop_id,
    configuration."Id"   AS configuration_id,
    configuration."CampaignId" AS configuration_campaign_id
FROM commerce."ShopOffer" AS offer
JOIN item_catalog."ItemConfiguration" AS configuration
    ON configuration."Id" = offer."ItemConfigurationId"
WHERE offer."ItemConfigurationId" IS NOT NULL
  AND configuration."CampaignId" IS NOT NULL
  AND configuration."CampaignId" <> offer."CampaignId";

-- 2. Экземпляры предметов, ссылающиеся на конфигурации чужих кампаний.
SELECT
    item."InstanceKey"   AS instance_key,
    item."CampaignId"    AS item_campaign_id,
    configuration."Id"   AS configuration_id,
    configuration."CampaignId" AS configuration_campaign_id
FROM inventory."ItemInstance" AS item
JOIN item_catalog."ItemConfiguration" AS configuration
    ON configuration."Id" = item."ItemConfigurationId"
WHERE configuration."CampaignId" IS NOT NULL
  AND configuration."CampaignId" <> item."CampaignId";