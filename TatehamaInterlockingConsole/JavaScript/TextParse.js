/**
 * @summary てこの内容をパースする関数
 * @param {string} input パースする文字列
 * @param {string} depth 深さ
 * @param {string} station 所属駅
 * @param {string} kata 片鎖錠かどうか
 * @returns {Array} パース結果のオブジェクトの配列
 */
function TekoParse(input, depth = 0, station = 0, kata = false) {
    if (input == "" || input == " ") {
        return [];
    }
    loop++;
    //console.log(`${"　".repeat(depth)}TekoParse(${input}, ${depth}, ${station})@${loop}`);

    //意味区切り括弧・所属駅括弧のうち速い方を見つける。
    const resultIndex = findLastTargetIndex(input, ["}", "]", "但"]);
    if (resultIndex == 0) {
        //意味区切り括弧
        //console.log(`${"　".repeat(depth)}意味区切り括弧分割`);
        let r = splitStringAtTwoTargets(input, "{", "}");
        if (r["position1"] != -1) {
            //前中後すべてに再帰的実行
            let b = TekoParse(r["before"], depth + 1, station);
            let w = TekoParse(r["between"], depth + 1, station);
            let a = TekoParse(r["after"], depth + 1, station);
            return [...b, ...w, ...a];
        }
    }
    else if (resultIndex == 1) {
        //所属駅括弧
        //console.log(`${"　".repeat(depth)}所属駅括弧分割`);
        const count = countConsecutiveTargetsFromEnd(input, "]")["count"];
        //console.log(`${"　".repeat(depth)} count${count}`);
        let r = splitStringAtTwoTargets(input, "[".repeat(count), "]".repeat(count));
        if (r["position1"] != -1) {
            //前中後すべてに再帰的実行
            let b = TekoParse(r["before"], depth + 1, station);
            let w = TekoParse(r["between"], depth + 1, station + count);
            let a = TekoParse(r["after"], depth + 1, station);
            return [...b, ...w, ...a];
        }
    }
    else if (resultIndex == 2) {
        //但文節
        //console.log(`${"　".repeat(depth)}但条件`);
        let r = splitStringAtTarget(input, "但")
        let b = TekoParse(r["before"], depth + 1, station);
        let a = TekoParse(r["after"], depth + 1, station);
        //console.log(b);
        //但の前のexecuteに後ろを入れる
        if (b[0].type == "or") {
            //console.log(b[0].execute);
            b[0].execute.forEach((e) => {
                e.execute = a;
            })
        }
        else {
            b.forEach((e) => {
                e.execute = a;
            })
        }
        return b;
    }
    else {
        //意味区切り系括弧等がない状態まで分割した
        if (input.indexOf("又は") != -1) {
            //console.log(`${"　".repeat(depth)}又は条件分割`);
            //又はがあるとき
            let array = applyFunctionToArray(input.split("又は"), TekoParse, depth + 1, station);
            return [{
                type: "or",
                name: input,
                execute: array
            }];
        }
        else if (input.indexOf(" ") != -1) {
            //console.log(`${"　".repeat(depth)}且つ条件分割`);
            let array = applyFunctionToArray(input.split(" "), TekoParse, depth + 1, station);
            return array.flat(Infinity);
        }
        else if (input.indexOf(">") != -1) {
            //console.log(`${"　".repeat(depth)}片鎖錠`);
            let r = splitStringAtTwoTargets(input, "<", ">");
            if (r["position1"] != -1) {
                //前中後すべてに再帰的実行
                let b = TekoParse(r["before"], depth + 1, station);
                let w = TekoParse(r["between"], depth + 1, station, true);
                let a = TekoParse(r["after"], depth + 1, station);
                return [...b, ...w, ...a];
            }
        }
        else if (input.indexOf(")(") != -1) {
            //console.log(`${"　".repeat(depth)}反位 or 総括制御が複数`);
            const count = countConsecutiveTargetsFromEnd(input, "(")["count"];
            let r = splitStringAtTwoTargets(input, "(".repeat(count), ")".repeat(count));
            if (r["position1"] != -1) {
                //前中後すべてに再帰的実行
                let b = TekoParse(r["before"], depth + 1, station);
                let w = TekoParse("(".repeat(count) + r["between"] + ")".repeat(count), depth + 1, station);
                let a = TekoParse(r["after"], depth + 1, station);
                return [...b, ...w, ...a];
            }
        }
        else {
            //てこ・回路単体
            //console.log(`${"　".repeat(depth)}要素単体`);
            let teihan = false;
            let sokatsu = false;
            if (input.indexOf("((") != -1) {
                //console.log(`${"　".repeat(depth)} 総括制御`);
                sokatsu = true;
                input = input.replace("((", "").replace("))", "")
            }
            else if (input.indexOf("(") != -1) {
                //console.log(`${"　".repeat(depth)} 反位`);
                teihan = true;
                input = input.replace("(", "").replace(")", "")
            }
            else {
                //console.log(`${"　".repeat(depth)} 定位・その他`);
            }

            //タイプ確認
            var type = "Null";
            if (window.SaveData.TekoTypeObj[input]) {
                type = window.SaveData.TekoTypeObj[input].type;
            }
            else if (input.endsWith("T")) {
                type = "Track";
            }
            else if (input.endsWith("秒")) {
                type = "Timer";
                input = input.replace("秒", "")
            }

            return [{
                type: type,
                station: station,
                name: input,
                teihan: teihan,
                execute: [],
                sokatsu: sokatsu,
                kata: kata
            }];
        }
    }
}

/**
 * 進路区分鎖錠に対応するパース
 */
function routeLockParse(input) {
    let r = [];
    if (input.startsWith('(') && input.endsWith(')')) {
        input = input.slice(1, -1);
    }
    console.log(input);
    input = input.replace(/\) \(/g, ")(");
    console.log(input);
    input.split(')(').forEach((e) => {
        r.push(TekoParse(e));
    })
    return r;
}

/**
 * 配列のすべての要素に対して特定の関数を適用する関数
 * @param {Array} array - 元の配列
 * @param {function} func - 適用する関数
 * @param  {...any} args - 適用する関数に渡す追加の引数
 * @returns {Array} - 新しい配列
 */
function applyFunctionToArray(array, func, ...args) {
    return array.map(element => func(element, ...args)).filter(element => element !== null);
}

/**
 * 元の文字列と対象文字列の配列を受け取り、対象文字列のうち最後から数えて最も早く出現する文字列のインデックスを返す関数
 * @param {string} str - 元の文字列
 * @param {string[]} targets - 対象文字列の配列
 * @returns {number} - 最後から数えて最も早く出現する対象文字列のインデックス、見つからなかった場合は-1
 */
function findLastTargetIndex(str, targets) {
    let latestIndex = -1;
    let targetIndex = -1;

    for (let i = 0; i < targets.length; i++) {
        const index = str.lastIndexOf(targets[i]);
        if (index !== -1 && (latestIndex === -1 || index > latestIndex)) {
            latestIndex = index;
            targetIndex = i;
        }
    }

    return targetIndex;
}


/**
 * 指定された文字列が最後に発見された箇所で、その文字列が何回連続で登場するかをカウントする関数
 * @param {string} str - 元の文字列
 * @param {string} target - 対象の文字列
 * @returns {object} - 最後に発見された箇所と連続回数
 */
function countConsecutiveTargetsFromEnd(str, target) {
    // targetの位置を探す
    const position = str.lastIndexOf(target);

    // targetが見つからなかった場合
    if (position === -1) {
        return {
            position: -1,
            count: 0
        };
    }

    let count = 0;
    let i = position;

    // 連続するtargetの数をカウント
    while (i >= 0 && str.substr(i, target.length) === target) {
        count++;
        i -= target.length;
    }

    return {
        position: position,
        count: count
    };
}

/**
 * 文字列内にある特定の文字列を最後から探し出し、その文字の位置を知り、文字列をその位置で分割する関数
 * @param {string} str - 対象の文字列
 * @param {string} target - 探す特定の文字列
 * @returns {object} - 分割された文字列と位置を含むオブジェクト
 */
function splitStringAtTarget(str, target) {
    // 特定文字列の最後の位置を探す
    const position = str.lastIndexOf(target);

    // 見つからなかった場合
    if (position === -1) {
        return {
            before: str,
            target: null,
            after: null,
            position: -1
        };
    }

    // 文字列を分割
    const before = str.substring(0, position);
    const after = str.substring(position + target.length);

    return {
        before: before,
        target: target,
        after: after,
        position: position
    };
}

/**
 * 文字列内にある2つの特定の文字列を最後から探し出し、それらの文字の位置を知り、文字列をその位置で分割する関数
 * @param {string} str - 対象の文字列
 * @param {string} target1 - 最初に探す特定の文字列
 * @param {string} target2 - 次に探す特定の文字列
 * @returns {object} - 分割された文字列と位置を含むオブジェクト
 */
function splitStringAtTwoTargets(str, target1, target2) {
    // target2の位置を最後から探す
    const position2 = str.lastIndexOf(target2);

    // target2が見つからなかった場合
    if (position2 === -1) {
        return {
            before: str,
            between: null,
            after: null,
            position1: -1,
            position2: -1
        };
    }

    // target1の位置をtarget2より前で最後から探す
    const position1 = str.lastIndexOf(target1, position2 - target1.length);

    // target1が見つからなかった場合
    if (position1 === -1) {
        return {
            before: str.substring(0, position2),
            between: null,
            after: str.substring(position2 + target2.length),
            position1: -1,
            position2: position2
        };
    }

    // 文字列を分割
    const before = str.substring(0, position1);
    const between = str.substring(position1 + target1.length, position2);
    const after = str.substring(position2 + target2.length);

    return {
        before: before,
        between: between,
        after: after,
        position1: position1,
        position2: position2
    };
}

var loop = 0;