CREATE VIEW ViewMainPrices AS
SELECT 
	ViewMainPeriod.Year,
	ViewMainPeriod.Month,
	ViewMainPeriod.IdCostCenter,
	ViewMainPeriod.IdEnergyResource,  
	MAX(ViewMainPeriod.Plan) AS Plan, 
	MAX(ViewMainPeriod.Fact) AS Fact,
	SUM("EnergyResourceNorms".NormWinter * "Tariffs".Tariff) AS CostWinter,
	SUM("EnergyResourceNorms".NormSummer * "Tariffs".Tariff) AS CostSummer
FROM ViewMainPeriods
JOIN "EnergyResourceNorms" ON  ViewMainPeriods.IdEnergyResource= EnergyResourceNorms.IdProduct
JOIN "Tariffs" ON "EnergyResourceNorms".IdER = "Tariffs".Id
GROUP BY Year, Month, IdCostCenter, IdEnergyResource