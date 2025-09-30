using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CameraManager cameraManager;
    public RoadManager roadManager;
    public InputManager inputManager;

    public UIController uiController;

    public StructureManager structureManager;

    private void Start()
    {
        uiController.OnRoadPlacement += RoadPlacementHandler;
        uiController.OnHousePlacement += HousePlacementHandler;
        uiController.OnClear += ClearInputActions;
        uiController.OnSpecialPlacement += SpecialPlacementHandler;
        uiController.OnBigStructurePlacement += BigStructurePlacementHandler;

    }

    private void BigStructurePlacementHandler(int index, int width, int height)
    {
        ClearInputActions();

        inputManager.OnMouseClick += (location) => structureManager.PlaceBigStructure(location, width, height, index);
    }

    private void SpecialPlacementHandler()
    {
        ClearInputActions();

        inputManager.OnMouseClick += structureManager.PlaceSpecial;
    }

    private void HousePlacementHandler(int houseNum, bool isAi)
    {
        ClearInputActions();

        // inputManager.OnMouseUpWithLocation += (location) => structureManager.PlaceHouseBuffered(location, houseNum);
        inputManager.OnMouseUpWithLocation += (location) => structureManager.PlaceHouseBufferedDelayed(location, houseNum, isAi);
    }

    private void RoadPlacementHandler()
    {
        ClearInputActions();
        cameraManager.cameraDragEnabled = false;
        inputManager.OnMouseClick += roadManager.PlaceRoad;
        inputManager.OnMouseHold += roadManager.PlaceRoad;
        inputManager.OnMouseUp += roadManager.FinishPlacingRoad;
    }

    public void ClearInputActions()
    {
        inputManager.OnMouseClick = null;
        inputManager.OnMouseHold = null;
        inputManager.OnMouseUp = null;
        inputManager.OnMouseUpWithLocation = null;
        // cameraManager.cameraDragEnabled = true;

    }

}
