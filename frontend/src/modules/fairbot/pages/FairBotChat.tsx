import React from "react";

import {
    styled,
    Box,
    Paper,
    Grid
} from '@mui/material';

// import { useFairBot } from "../hooks/useFairBot";
// import { useResultsPanel } from "../hooks/useResultsPanel";


const Item = styled(Paper)(({ theme }) => ({
    backgroundColor: '#fff',
    ...theme.typography.body2,
    padding: theme.spacing(1),
    textAlign: 'center',
    color: (theme.vars ?? theme).palette.text.secondary,
    ...theme.applyStyles('dark', {
        backgroundColor: '#1A2027',
    }),
}));

export const FairBotChat = () => {
    // const { messages, sendMessage, isLoading } = useFairBot();
    // const { currentResult } = useResultsPanel();

    return (
        <Box sx={{ flexGrow: 1 }}>
            <Grid container spacing={0}>
                <Grid size={2}>
                    <Grid>
                        <Item>Top</Item>
                    </Grid>
                    <Grid>
                        <Item>center</Item>
                    </Grid>
                    <Grid>
                        <Item>Bottom</Item>
                    </Grid>
                </Grid>
                <Grid size={7}>
                    <Grid>
                        <Item>Top</Item>
                    </Grid>
                    <Grid>
                        <Item>center</Item>
                    </Grid>
                    <Grid>
                        <Item>Bottom</Item>
                    </Grid>
                </Grid>
                <Grid size={3}>
                    <Item>right</Item>
                </Grid>
            </Grid>
        </Box>
    );

};

// {
//     currentResult?(

//     ): (

//         )}