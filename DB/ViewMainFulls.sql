CREATE VIEW ViewMainFulls AS
SELECT 
	ViewMainPrices.Year,
	ViewMainPrices.Month,
	ViewMainPrices.IdCostCenter,
	CostCenters.IdDepart,
	Departs.IdCategory,
	Departs.IsAnalysis,
	DepartCategories.Name,
	ViewMainPrices.IdEnergyResource, 
	EnergyResources.IsMain,
	EnergyResources.Name,
	Units.Name,
	ViewMainPrices.Plan, 
	ViewMainPrices.Fact,
	(Fact - Plan) AS Difference,
	(ViewMainPrices.CostWinter*Plan) AS PlanWinter,
	(ViewMainPrices.CostSummer*Plan) AS PlanSummer,
	(ViewMainPrices.CostWinter*Fact) AS FactWinter,
	(ViewMainPrices.CostSummer*Fact) AS FactSummer,
	(ViewMainPrices.CostWinter*Fact-ViewMainPrices.CostWinter*Plan) AS DifferenceWinter,
	(ViewMainPrices.CostSummer*Fact-ViewMainPrices.CostSummer*Plan) AS DifferenceSummer
FROM ViewMainPrices
JOIN EnergyResources ON ViewMainPrices.IdEnergyResource = EnergyResources.Id
JOIN Units ON EnergyResources.Unit = Units.Id
JOIN CostCenters ON ViewMainPrices.IdCostCenter = CostCenters.Id
JOIN Departs ON CostCenters.IdDepart = Departs.Id
JOIN DepartCategories ON Departs.IdCategory = DepartCategories.Id
ORDER BY Year, Month, IdCostCenter, ViewMainPrices.IdEnergyResource