# Roll-a-Ball Shapes（課題②）

## 動作動画

[Roll-a-Ball 課題①・②の動作動画（YouTube）](https://youtu.be/DwcY27HnhtA)

前半は立方体の基本版、後半は三角柱・円錐版です。

## 開き方

Unity Hub の「追加」からこのフォルダを選び、Unity 6000.6.6f1 で開いてください。
`Assets/Scenes/SampleScene.unity` を開いて Play を押します。
元プロジェクトの移動操作、カメラ追従、スコア・経過時間表示、全取得時のクリア表示を引き継ぎます。

## 変更内容

- 元の12個の立方体アイテムを、黄色の三角柱6個・オレンジ色の円錐6個に変更。
- 配置と大きさは元のシーンから引き継ぎ、開始時は形が分かる向きに統一。
- 三角柱・円錐ごとにメッシュ、マテリアル、Prefabを保存。
- 形状に沿う凸MeshColliderをTriggerとして使用。プレイヤーが触れると消え、1点加算。
- `Assets/Shapes/` に形状データ、`Assets/PreFabs/` に各Prefabがあります。

`Library`、`Temp`、`Logs`などの生成データは元からコピーせず、このプロジェクト専用に再生成します。
元プロジェクトは変更していません。

